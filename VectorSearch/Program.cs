using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;

using VectorSearch;


var ollamaEndpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434";
//var chatModelName = "llama3.2";
//var embeddingModelName2 = "ollama pull nomic-embed-text-v2-moe";
var embeddingModelName = "all-minilm";


// create an embedding genetator using OllamaSharp client and the specified embedding model
IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator = new OllamaSharp.OllamaApiClient(new Uri(ollamaEndpoint), embeddingModelName);

// create a vector store using the embedding generator and specify the distance function for similarity search
var vectorStore = new InMemoryVectorStore();
var movieVectorStore = vectorStore.GetCollection<int,Movie>("Movies");
await movieVectorStore.EnsureCollectionExistsAsync();

// create a list of movies to be added to the vector store
foreach (var movie in MovieData.Movies)
{
    // generate the embedding vector for the movie description using the embedding generator
    var embedding = await embeddingGenerator.GenerateVectorAsync(movie.Description);
    movie.Vector = embedding;
    // add the movie to the vector store
    await movieVectorStore.UpsertAsync( movie);
}

var query = "A movie about a computer hacker who discovers the true nature of reality.";
var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

var searchResults =  movieVectorStore.SearchAsync(queryEmbedding, 2);

await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}, Description: {result.Record.Description}");
}
