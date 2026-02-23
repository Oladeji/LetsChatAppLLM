using LetsChatApp.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LetsChatAppBackEnd.Services
{
    public class SerchingEntityService
    {
        private readonly IChatClient _chatClient;
        private readonly VectorStoreCollection<string, EntityVectorStore> _vectorStoreCollection;   
        private readonly ILogger<SerchingEntityService> _logger;
        private readonly IEmbeddingGenerator _embeddingGenerator;
     


        public SerchingEntityService(IChatClient chatClient, VectorStoreCollection<string, EntityVectorStore> vectorStoreCollection, ILogger<SerchingEntityService> logger, IEmbeddingGenerator embeddingGenerator)
        {
            _chatClient = chatClient;
            _vectorStoreCollection = vectorStoreCollection;
            _logger = logger;
            _embeddingGenerator = embeddingGenerator;
        }

        public Task InitEmbeddding() 
        {
            _vectorStoreCollection.EnsureCollectionExistsAsync();
            //This where we add our data into the vectordb
            //like the way we have IngestDataAsync in the DataIngestor class, we can have a method that adds data into the vector store collection,
            //and then we can call that method here to add data into the vector store collection.
            // since rely this will be a kind of class you have in the upload data  section
            // it will be responsible for adding the data into the vector store collection, and then we can call that method here to add data into the vector store collection.


            return Task.CompletedTask;
        
        }
    }
}
