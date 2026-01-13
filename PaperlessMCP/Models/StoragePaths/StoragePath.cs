using System.Text.Json.Serialization;

namespace PaperlessMCP.Models.StoragePaths;

/// <summary>
/// Represents a storage path in Paperless-ngx.
/// </summary>
public record StoragePath
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("slug")]
    public string Slug { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; init; } = string.Empty;

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int MatchingAlgorithm { get; init; }

    [JsonPropertyName("document_count")]
    public int DocumentCount { get; init; }

    [JsonPropertyName("owner")]
    public int? Owner { get; init; }
}

/// <summary>
/// Request to create a new storage path.
/// </summary>
public record StoragePathCreateRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int? MatchingAlgorithm { get; init; }
}

/// <summary>
/// Request to update an existing storage path.
/// </summary>
public record StoragePathUpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("path")]
    public string? Path { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int? MatchingAlgorithm { get; init; }
}
