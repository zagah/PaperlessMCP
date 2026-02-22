using System.Text.Json.Serialization;

namespace PaperlessMCP.Models.Documents;

/// <summary>
/// Request for bulk document operations.
/// </summary>
public record BulkEditRequest
{
    [JsonPropertyName("documents")]
    public required int[] Documents { get; init; }

    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("parameters")]
    public object? Parameters { get; init; }
}
