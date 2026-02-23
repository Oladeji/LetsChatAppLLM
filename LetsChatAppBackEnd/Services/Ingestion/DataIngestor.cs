using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LetsChatAppBackEnd.Services.Ingestion;

public class DataIngestor(
    ILogger<DataIngestor> logger,
    VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
{
    public static async Task IngestDataAsync(IServiceProvider services, IIngestionSource source)
    {
        using var scope = services.CreateScope();
        var ingestor = scope.ServiceProvider.GetRequiredService<DataIngestor>();
        await ingestor.IngestDataAsync(source);
    }

    public async Task IngestDataAsync(IIngestionSource source)
    {
        try {
        string chunkCollectionName = "chunks";
        string documentCollectionName = "docs";
        var chunksCollection = vectorStore.GetCollection<Guid, IngestedChunk>(chunkCollectionName);
        var documentsCollection = vectorStore.GetCollection<Guid, IngestedDocument>(documentCollectionName);

        await chunksCollection.EnsureCollectionExistsAsync();
        await documentsCollection.EnsureCollectionExistsAsync();

        var sourceId = source.SourceId;
        var documentsForSource = await documentsCollection.GetAsync(doc => doc.SourceId == sourceId, top: int.MaxValue).ToListAsync();

        var deletedDocuments = await source.GetDeletedDocumentsAsync(documentsForSource);
        foreach (var deletedDocument in deletedDocuments)
        {
            logger.LogInformation("Removing ingested data for {DocumentId}", deletedDocument.DocumentId);
            await DeleteChunksForDocumentAsync(deletedDocument, chunksCollection);
            await documentsCollection.DeleteAsync(deletedDocument.Key);
        }

        var modifiedDocuments = await source.GetNewOrModifiedDocumentsAsync(documentsForSource);
        foreach (var modifiedDocument in modifiedDocuments)
        {
            logger.LogInformation("Processing {DocumentId}", modifiedDocument.DocumentId);
            await DeleteChunksForDocumentAsync(modifiedDocument, chunksCollection);

            await documentsCollection.UpsertAsync(modifiedDocument);

            var newChunks = await source.CreateChunksForDocumentAsync(modifiedDocument);

            // Generate embeddings for each chunk
            var chunksWithEmbeddings = new List<IngestedChunk>();
                //   results.Add(new() { Key = Guid.CreateVersion7().ToString(), SourceId = SourceId, DocumentId = sourceFileId, DocumentVersion = sourceFileVersion });

                foreach (var chunk in newChunks)
            {
                var embedding = await embeddingGenerator.GenerateAsync(chunk.Text);
                chunk.Vector = embedding.Vector;
                chunksWithEmbeddings.Add(chunk);
            }

            await chunksCollection.UpsertAsync(chunksWithEmbeddings);
        }

        logger.LogInformation("Ingestion is up-to-date");
        }
        catch  (Exception x) {
            Console.WriteLine(x);
            throw ;
        }
    }

    private async Task DeleteChunksForDocumentAsync(IngestedDocument document, VectorStoreCollection<Guid, IngestedChunk> chunksCollection)
    {
        var documentId = document.DocumentId;
        var chunksToDelete = await chunksCollection.GetAsync(record => record.DocumentId == documentId, int.MaxValue).ToListAsync();
        if (chunksToDelete.Count != 0)
        {
            await chunksCollection.DeleteAsync(chunksToDelete.Select(r => r.Key));
        }
    }
}

/// <summary>
/// DataIngestor that directly injects VectorStoreCollection instances instead of using VectorStore
/// </summary>
public class DataIngestor2(
    ILogger<DataIngestor2> logger,
    VectorStoreCollection<Guid, IngestedChunk> chunksCollection,
    VectorStoreCollection<Guid, IngestedDocument> documentsCollection,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
{
    public static async Task IngestDataAsync(IServiceProvider services, IIngestionSource source)
    {
        using var scope = services.CreateScope();
        var ingestor = scope.ServiceProvider.GetRequiredService<DataIngestor2>();
        await ingestor.IngestDataAsync(source);
    }

    public async Task IngestDataAsync(IIngestionSource source)
    {
        await chunksCollection.EnsureCollectionExistsAsync();
        await documentsCollection.EnsureCollectionExistsAsync();

        var sourceId = source.SourceId;
        var documentsForSource = await documentsCollection.GetAsync(doc => doc.SourceId == sourceId, top: int.MaxValue).ToListAsync();

        var deletedDocuments = await source.GetDeletedDocumentsAsync(documentsForSource);
        foreach (var deletedDocument in deletedDocuments)
        {
            logger.LogInformation("Removing ingested data for {DocumentId}", deletedDocument.DocumentId);
            await DeleteChunksForDocumentAsync(deletedDocument);
            await documentsCollection.DeleteAsync(deletedDocument.Key);
        }

        var modifiedDocuments = await source.GetNewOrModifiedDocumentsAsync(documentsForSource);
        foreach (var modifiedDocument in modifiedDocuments)
        {
            logger.LogInformation("Processing {DocumentId}", modifiedDocument.DocumentId);
            await DeleteChunksForDocumentAsync(modifiedDocument);

            await documentsCollection.UpsertAsync(modifiedDocument);

            var newChunks = await source.CreateChunksForDocumentAsync(modifiedDocument);

            // Generate embeddings for each chunk
            var chunksWithEmbeddings = new List<IngestedChunk>();
            foreach (var chunk in newChunks)
            {
                var embedding = await embeddingGenerator.GenerateAsync(chunk.Text);
                chunk.Vector = embedding.Vector;
                chunksWithEmbeddings.Add(chunk);
            }

            await chunksCollection.UpsertAsync(chunksWithEmbeddings);
        }

        logger.LogInformation("Ingestion is up-to-date");
    }

    private async Task DeleteChunksForDocumentAsync(IngestedDocument document)
    {
        var documentId = document.DocumentId;
        var chunksToDelete = await chunksCollection.GetAsync(record => record.DocumentId == documentId, int.MaxValue).ToListAsync();
        if (chunksToDelete.Count != 0)
        {
            await chunksCollection.DeleteAsync(chunksToDelete.Select(r => r.Key));
        }
    }
}
