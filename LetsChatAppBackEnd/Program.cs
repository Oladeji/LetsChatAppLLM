using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using LetsChatAppBackEnd.Services;
using LetsChatAppBackEnd.Services.Ingestion;
using LetsChatAppBackEnd.Hubs;
using LetsChatAppBackEnd.Models;
using OllamaSharp;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure AI services
IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(new Uri("http://localhost:11434"), "all-minilm");

// Configure vector store
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";

builder.Services.AddSqliteCollection<string, IngestedChunk>("data-letschatapp-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedDocument>("data-letschatapp-documents", vectorStoreConnectionString);

builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

// Configure upload directory
var uploadDirectory = Path.Combine(AppContext.BaseDirectory, "Uploads");
if (!Directory.Exists(uploadDirectory))
{
    Directory.CreateDirectory(uploadDirectory);
}
builder.Services.AddSingleton(new PDFUploadSource(uploadDirectory));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub").RequireCors("AllowReactApp");

// API Endpoints
app.MapPost("/api/search", async (SearchRequest request, SemanticSearch search) =>
{
    var results = await search.SearchAsync(request.Query, request.DocumentIdFilter, request.MaxResults);
    return Results.Ok(results.Select(r => new SearchResult(r.DocumentId, r.PageNumber, r.Text)));
})
.WithName("SearchDocuments")
.WithOpenApi();

app.MapGet("/api/documents", async (VectorStoreCollection<string, IngestedDocument> documentsCollection) =>
{
    var documents = await documentsCollection.GetAsync(_ => true, top: int.MaxValue).ToListAsync();
    return Results.Ok(documents.Select(d => new DocumentInfo(d.DocumentId, d.SourceId, d.DocumentVersion)));
})
.WithName("GetDocuments")
.WithOpenApi();

app.MapDelete("/api/documents/{documentId}", async (
    string documentId,
    VectorStoreCollection<string, IngestedDocument> documentsCollection,
    VectorStoreCollection<string, IngestedChunk> chunksCollection) =>
{
    var documents = await documentsCollection.GetAsync(d => d.DocumentId == documentId, top: int.MaxValue).ToListAsync();
    if (documents.Count == 0)
    {
        return Results.NotFound(new { Message = "Document not found" });
    }

    foreach (var doc in documents)
    {
        var chunks = await chunksCollection.GetAsync(c => c.DocumentId == documentId, top: int.MaxValue).ToListAsync();
        if (chunks.Count > 0)
        {
            await chunksCollection.DeleteAsync(chunks.Select(c => c.Key));
        }
        await documentsCollection.DeleteAsync(doc.Key);
    }

    return Results.Ok(new { Message = "Document deleted successfully" });
})
.WithName("DeleteDocument")
.WithOpenApi();

app.MapPost("/api/documents/upload", async (
    IFormFile file,
    PDFUploadSource uploadSource,
    IServiceProvider services) =>
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
})
.WithName("UploadDocument")
.DisableAntiforgery()
.WithOpenApi();

app.MapPost("/api/documents/reingest", async (IServiceProvider services, PDFUploadSource uploadSource) =>
{
    await DataIngestor.IngestDataAsync(services, uploadSource);
    return Results.Ok(new { Message = "Re-ingestion completed successfully" });
})
.WithName("ReingestDocuments")
.WithOpenApi();

app.MapGet("/api/documents/view/{documentId}", async (string documentId, PDFUploadSource uploadSource) =>
{
    var uploadDir = uploadSource.GetUploadDirectory();
    var filePath = Path.Combine(uploadDir, documentId);

    if (!File.Exists(filePath))
    {
        return Results.NotFound(new { Message = "Document not found" });
    }

    var fileStream = File.OpenRead(filePath);
    return Results.File(fileStream, "application/pdf", documentId);
})
.WithName("ViewDocument")
.WithOpenApi();

// Initial ingestion from uploads directory
await DataIngestor.IngestDataAsync(app.Services, app.Services.GetRequiredService<PDFUploadSource>());

app.Run();
