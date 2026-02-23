using Microsoft.Extensions.VectorData;

namespace LetsChatAppBackEnd.Services;

public class IngestedChunk
{
    public const int VectorDimensions = 768; // 768 for nomic-embed-text embedding model
    public const string VectorDistanceFunction = "CosineSimilarity";
    public const string CollectionName = "data-IngestedChunk";
    [VectorStoreKey]
    //public required string Key { get; set; }
    public required Guid Key { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public required string DocumentId { get; set; }

    [VectorStoreData]
    public int PageNumber { get; set; }

    [VectorStoreData]
    public required string Text { get; set; }

    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    //public string? Vector => Text;
    public ReadOnlyMemory<float> Vector { get; set; } = new ReadOnlyMemory<float>([0, 0]);
}
