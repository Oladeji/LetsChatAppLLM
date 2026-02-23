using Microsoft.Extensions.VectorData;

namespace LetsChatAppBackEnd.Services.Ingestion;

public class IngestedDocument
{
    private const int VectorDimensions = 2;
    public const string VectorDistanceFunction = DistanceFunction.CosineSimilarity;
    public const string CollectionName = "data-IngestedDocument";

    [VectorStoreKey]


    //public required string Key { get; set; }

    public required Guid Key{ get; set; }
    [VectorStoreData(IsIndexed = true)]
    public required string SourceId { get; set; }

    [VectorStoreData]
    public required string DocumentId { get; set; }

    [VectorStoreData]
    public required string DocumentVersion { get; set; }

    // The vector is not used but required for some vector databases
    [VectorStoreVector(2, DistanceFunction = VectorDistanceFunction)]
    public ReadOnlyMemory<float> Vector { get; set; } = new ReadOnlyMemory<float>([0, 0]);
}
