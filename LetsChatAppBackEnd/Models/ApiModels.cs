namespace LetsChatAppBackEnd.Models;

public record SearchRequest(string Query, string? DocumentIdFilter = null, int MaxResults = 5);

public record SearchResult(string DocumentId, int PageNumber, string Text);

public record DocumentInfo(string DocumentId, string SourceId, string DocumentVersion);

public record UploadResponse(bool Success, string Message, string? DocumentId = null);
