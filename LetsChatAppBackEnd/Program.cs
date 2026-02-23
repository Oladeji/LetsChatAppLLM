using LetsChatApp.Models;
using LetsChatAppBackEnd;
using LetsChatAppBackEnd.Services;
using LetsChatAppBackEnd.Services.Ingestion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OllamaSharp;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// Configure CORS for React frontend

string[] allowedOrigins = ["http://localhost:5173", "http://localhost:3000"];
var lllmEndPoint = "http://localhost:11434";
var llmModelName = "llama3.2";
var embeddingModelname = "all-minilm";//// distance function for nomic is 768
var actualDocumentsPath = "AllDbDocumentsUploads";

var qdrantEndpoint = "http://localhost:6333"; 
var qdrantEmbeddingModel = "nomic-embed-text";//ollama pull nomic-embed-text
//var chuncksCollectName = "Chunks";
//var documentCollectName = "documents";

string chunkCollectionName = "chunks";
string documentCollectionName = "docs";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure AI services
IChatClient chatClient = new OllamaApiClient(new Uri(lllmEndPoint), llmModelName);
//IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(new Uri(lllmEndPoint), embeddingModelname);

IEmbeddingGenerator<string, Embedding<float>> qdEmbeddingGenerator = new OllamaApiClient(new Uri(lllmEndPoint), qdrantEmbeddingModel);



// Configure vector store for sql lite
//var vectorStorePath = Path.Combine(AppContext.BaseDirectory, sqlLitePath);
//var vectorStoreConnectionString = $"Data Source={vectorStorePath}";

//builder.Services.AddSqliteCollection<string, IngestedChunk>("data-letschatapp-chunks", vectorStoreConnectionString);
//builder.Services.AddSqliteCollection<string, IngestedDocument>("data-letschatapp-documents", vectorStoreConnectionString);

// Configure vector store for qdrant
var qdrantClient = new QdrantClient("localhost",6334);
builder.Services.AddSingleton(qdrantClient);
builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(qdEmbeddingGenerator);
VectorStore vectorStore = new QdrantVectorStore(qdrantClient, false, new QdrantVectorStoreOptions
{ 
    EmbeddingGenerator = qdEmbeddingGenerator,
});

builder.Services.AddSingleton(vectorStore);


builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();

// Option A: Collections will be retrieved from VectorStore which already has embedding generator configured
// (Used by DataIngestor and SemanticSearch)

// Option B: Register collections directly with embedding generator
// (Used by DataIngestor2 and SemanticSearch2)
builder.Services.AddSingleton<VectorStoreCollection<Guid, IngestedChunk>>(sp =>
{
    var client = sp.GetRequiredService<QdrantClient>();
    var embGen = sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

    return new QdrantCollection<Guid, IngestedChunk>(
        client, 
        "chunks", 
        ownsClient: false,
        new QdrantCollectionOptions
        {
            EmbeddingGenerator = embGen
        });
});

builder.Services.AddSingleton<VectorStoreCollection<Guid, IngestedDocument>>(sp =>
{
    var client = sp.GetRequiredService<QdrantClient>();
    var embGen = sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

    return new QdrantCollection<Guid, IngestedDocument>(
        client, 
        "docs", 
        ownsClient: false,
        new QdrantCollectionOptions
        {
            EmbeddingGenerator = embGen
        });
});




builder.Services.AddScoped<DataIngestor>();
builder.Services.AddScoped<DataIngestor2>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddSingleton<SemanticSearch2>();

//builder.Services.AddQdrantVectorStore(vectorStorePath);
//builder.Services.AddQdrantClient("drantdb");

// Configure upload directory
var uploadDirectory = Path.Combine(AppContext.BaseDirectory, actualDocumentsPath);
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

//app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.MapLetsChatAppEndPoints();
// Initial ingestion from uploads directory
await DataIngestor.IngestDataAsync(app.Services, app.Services.GetRequiredService<PDFUploadSource>());

app.Run();
