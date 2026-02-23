using LetsChatAppBackEnd;
using LetsChatAppBackEnd.Configuration;
using LetsChatAppBackEnd.Services;
using LetsChatAppBackEnd.Services.Ingestion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OllamaSharp;
using Qdrant.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOriginsPolicy", policy =>
    {
        policy.WithOrigins(appSettings.Cors.AllowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure AI services
IChatClient chatClient = new OllamaApiClient(new Uri(appSettings.Ollama.Endpoint), appSettings.Ollama.ChatModelName);
IEmbeddingGenerator<string, Embedding<float>> qdEmbeddingGenerator = new OllamaApiClient(new Uri(appSettings.Ollama.Endpoint), appSettings.Ollama.EmbeddingModelName);


// Configure vector store for qdrant
var qdrantClient = new QdrantClient(appSettings.Qdrant.Host, appSettings.Qdrant.Port);
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
// (Used by DataIngestor2, SemanticSearch2, and API endpoints)
//builder.Services.AddSingleton<VectorStoreCollection<Guid, IngestedChunk>>(sp =>
//{
//    var client = sp.GetRequiredService<QdrantClient>();
//    var embGen = sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

//    return new QdrantCollection<Guid, IngestedChunk>(
//        client, 
//        appSettings.Qdrant.Collections.Chunks, 
//        ownsClient: false,
//        new QdrantCollectionOptions
//        {
//            EmbeddingGenerator = embGen
//        });
//});

//builder.Services.AddSingleton<VectorStoreCollection<Guid, IngestedDocument>>(sp =>
//{
//    var client = sp.GetRequiredService<QdrantClient>();
//    var embGen = sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

//    return new QdrantCollection<Guid, IngestedDocument>(
//        client, 
//        appSettings.Qdrant.Collections.Documents, 
//        ownsClient: false,
//        new QdrantCollectionOptions
//        {
//            EmbeddingGenerator = embGen
//        });
//});




builder.Services.AddScoped<DataIngestor>();
//builder.Services.AddScoped<DataIngestor2>();
builder.Services.AddSingleton<SemanticSearch>();
//builder.Services.AddSingleton<SemanticSearch2>();


var uploadDirectory = Path.Combine(AppContext.BaseDirectory, appSettings.Documents.UploadPath);
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
app.UseCors("AllowedOriginsPolicy");
app.MapLetsChatAppEndPoints();
// Initial ingestion from uploads directory
//await DataIngestor.IngestDataAsync(app.Services, app.Services.GetRequiredService<PDFUploadSource>());

app.Run();
