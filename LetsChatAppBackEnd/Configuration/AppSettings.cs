namespace LetsChatAppBackEnd.Configuration;

public class AppSettings
{
    public CorsSettings Cors { get; set; } = new();
    public OllamaSettings Ollama { get; set; } = new();
    public QdrantSettings Qdrant { get; set; } = new();
    public DocumentSettings Documents { get; set; } = new();
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = [];
}

public class OllamaSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string ChatModelName { get; set; } = string.Empty;
    public string EmbeddingModelName { get; set; } = string.Empty;
}

public class QdrantSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public CollectionNames Collections { get; set; } = new();
}

public class CollectionNames
{
    public string Chunks { get; set; } = string.Empty;
    public string Documents { get; set; } = string.Empty;
}

public class DocumentSettings
{
    public string UploadPath { get; set; } = string.Empty;
}
