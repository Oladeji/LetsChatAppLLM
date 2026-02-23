using LetsChatAppBackEnd.Services.Ingestion;
using Microsoft.Extensions.VectorData;

namespace LetsChatAppBackEnd.Services;

public class SemanticSearch(VectorStore vectorStore)
{
    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        var vectorCollection = vectorStore.GetCollection<Guid, IngestedChunk>("chunks");

        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });

        return await nearest.Select(result => result.Record).ToListAsync();
    }
}

/// <summary>
/// SemanticSearch that directly injects VectorStoreCollection instead of using VectorStore.
/// The collection must be configured with an embedding generator to convert string queries to vectors.
/// </summary>
public class SemanticSearch2(VectorStoreCollection<Guid, IngestedChunk> vectorCollection)
{
    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });

        return await nearest.Select(result => result.Record).ToListAsync();
    }
}
