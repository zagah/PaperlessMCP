using System.Text.Json.Serialization;

namespace PaperlessMCP.Models.DocumentTypes;

/// <summary>
/// Represents a document type in Paperless-ngx.
/// </summary>
public record DocumentType
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

    [JsonPropertyName("owner")]
    public int? Owner { get; init; }
}

/// <summary>
/// Request to create a new document type.
/// </summary>
public record DocumentTypeCreateRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int? MatchingAlgorithm { get; init; }
}

/// <summary>
/// Request to update an existing document type.
/// </summary>
public record DocumentTypeUpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }

    [JsonPropertyName("matching_algorithm")]
    public int? MatchingAlgorithm { get; init; }
}
