using System.Text.Json.Serialization;

namespace PaperlessMCP.Models.Correspondents;

/// <summary>
/// Represents a correspondent in Paperless-ngx.
/// </summary>
public record Correspondent
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("slug")]
    public string Slug { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int MatchingAlgorithm { get; init; }

    [JsonPropertyName("document_count")]
    public int DocumentCount { get; init; }

    [JsonPropertyName("last_correspondence")]
    public DateTime? LastCorrespondence { get; init; }

    [JsonPropertyName("owner")]
    public int? Owner { get; init; }
}

/// <summary>
/// Request to create a new correspondent.
/// </summary>
public record CorrespondentCreateRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int? MatchingAlgorithm { get; init; }
}

/// <summary>
/// Request to update an existing correspondent.
/// </summary>
public record CorrespondentUpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int? MatchingAlgorithm { get; init; }
}
