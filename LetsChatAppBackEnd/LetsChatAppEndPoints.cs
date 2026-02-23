using LetsChatAppBackEnd.Hubs;
using LetsChatAppBackEnd.Models;
using LetsChatAppBackEnd.Services;
using LetsChatAppBackEnd.Services.Ingestion;
using Microsoft.Extensions.VectorData;


namespace LetsChatAppBackEnd
{
    public static class LetsChatAppEndPoints
    {

        public static void MapLetsChatAppEndPoints(this IEndpointRouteBuilder route)
        {

            // Map SignalR Hub
            route.MapHub<ChatHub>("/chatHub").RequireCors("AllowedOriginsPolicy");
            // API Endpoints
            route.MapPost("/api/search", searchRepo)
            .WithName("SearchDocuments")
            .RequireCors("AllowedOriginsPolicy");

            route.MapGet("/api/documents", GetDocs)
            .WithName("GetDocuments")
            .RequireCors("AllowedOriginsPolicy");

            route.MapDelete("/api/documents/{documentId}", DeleteDocs)
            .WithName("DeleteDocument")
            .RequireCors("AllowedOriginsPolicy");
            

            route.MapPost("/api/documents/upload",UploadDocs)
            .WithName("UploadDocument")
            .DisableAntiforgery()
            .RequireCors("AllowedOriginsPolicy");

            route.MapPost("/api/documents/reingest", ReIngestDocs)
            .WithName("ReingestDocuments")
            .RequireCors("AllowedOriginsPolicy");

            route.MapGet("/api/documents/view/{documentId}", ViewRepo)
            .WithName("ViewDocument")
            .RequireCors("AllowedOriginsPolicy");

        }

        public static async Task<IResult> ReIngestDocs(IServiceProvider services, PDFUploadSource uploadSource)
            {
            await DataIngestor.IngestDataAsync(services, uploadSource);
            return Results.Ok(new { Message = "Re-ingestion completed successfully" });
        }

        public static async Task<IResult> GetDocs(VectorStoreCollection<Guid, IngestedDocument> documentsCollection)
        {
            var documents = await documentsCollection.GetAsync(_ => true, top: int.MaxValue).ToListAsync();
            return Results.Ok(documents.Select(d => new DocumentInfo(d.DocumentId, d.SourceId, d.DocumentVersion)));
        }

        public static async Task<IResult> DeleteDocs(
                Guid documentId,
                VectorStoreCollection<Guid, IngestedDocument> documentsCollection,
                VectorStoreCollection<Guid, IngestedChunk> chunksCollection)
        { 
          
          //  var documents = await documentsCollection.GetAsync(d => d.DocumentId == documentId, top: int.MaxValue).ToListAsync();
            var documents = await documentsCollection.GetAsync(d => d.DocumentId.Equals(documentId), top: int.MaxValue).ToListAsync();
            if (documents.Count == 0)
            {
                return Results.NotFound(new { Message = "Document not found" });
            }

            foreach (var doc in documents)
            {
                var chunks = await chunksCollection.GetAsync(c => c.DocumentId.Equals( documentId), top: int.MaxValue).ToListAsync();
                //var chunks = await chunksCollection.GetAsync(c => c.DocumentId == documentId, top: int.MaxValue).ToListAsync();
                if (chunks.Count > 0)
                {
                    await chunksCollection.DeleteAsync(chunks.Select(c => c.Key));
                }
                await documentsCollection.DeleteAsync(doc.Key);
            }

            return Results.Ok(new { Message = "Document deleted successfully" });
        }

        public static async Task<IResult>UploadDocs( IFormFile file,PDFUploadSource uploadSource, IServiceProvider services) 
       {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new UploadResponse(false, "No file provided"));
            }

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new UploadResponse(false, "Only PDF files are allowed"));
            }

            var uploadDir = uploadSource.GetUploadDirectory();
            var filePath = Path.Combine(uploadDir, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await DataIngestor.IngestDataAsync(services, uploadSource);

            return Results.Ok(new UploadResponse(true, "File uploaded and ingested successfully", file.FileName));
        }

        public static async Task<IResult> ViewRepo(string documentId, PDFUploadSource uploadSource)
            {
            var uploadDir = uploadSource.GetUploadDirectory();
            var filePath = Path.Combine(uploadDir, documentId);

            if (!File.Exists(filePath))
            {
                return Results.NotFound(new { Message = "Document not found" });
            }

            var fileStream = File.OpenRead(filePath);
            return Results.File(fileStream, "application/pdf", documentId);
        }


       public static async Task<IResult> searchRepo(
            SearchRequest request,

            SemanticSearch semanticSearch)
        {
            var results = await semanticSearch.SearchAsync(request.Query, request.DocumentIdFilter, request.MaxResults);
            return Results.Ok(results.Select(r => new SearchResult(r.DocumentId, r.PageNumber, r.Text)));

        }


    }

}
