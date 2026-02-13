using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using LetsChatAppBackEnd.Services;
using LetsChatAppBackEnd.Services.Ingestion;

using OllamaSharp;
using Scalar.AspNetCore;
using LetsChatAppBackEnd;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// Configure CORS for React frontend

string[] allowedOrigins = ["http://localhost:5173", "http://localhost:3000"];
var lllmEndPoint = "http://localhost:11434";
var llmModelName = "llama3.2";
var embeddingModelname = "all-minilm";//// distance function for nomic is 768
var actualDocumentsPath = "AllDbDocumentsUploads";
var sqlLitePath = "vector-store.db";

var qdrantEndpoint= "http://localhost:6333"; 
var qdModelName = "gemma3:12b";
var qdrantEmbeddingModel = "nomic-embed-text";
var qdrantCollectName = "QdrantDb";

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
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(new Uri(lllmEndPoint), embeddingModelname);

IEmbeddingGenerator<string, Embedding<float>> qdEmbeddingGenerator = new OllamaApiClient(new Uri(lllmEndPoint), qdrantEmbeddingModel);



// Configure vector store for sql lite
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, sqlLitePath);
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";

builder.Services.AddSqliteCollection<string, IngestedChunk>("data-letschatapp-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedDocument>("data-letschatapp-documents", vectorStoreConnectionString);

// Configure vector store for quandrant
var qdrantClient = new QdrantClient(qdrantEndpoint);

var vectoreStore = new QdrantVectorStore(qdrantClient,new QdrantVectorStoreOptions 
{
    EmbeddingGenerator= qdEmbeddingGenerator 
}
);



builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

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

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.MapLetsChatAppEndPoints();
// Initial ingestion from uploads directory
await DataIngestor.IngestDataAsync(app.Services, app.Services.GetRequiredService<PDFUploadSource>());

app.Run();
