using System.Text.Json.Serialization;

namespace PaperlessMCP.Models.Common;

/// <summary>
/// Represents a paginated response from the Paperless-ngx API.
/// </summary>
/// <typeparam name="T">The type of items in the results.</typeparam>
public record PaginatedResult<T>
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("next")]
    public string? Next { get; init; }

    [JsonPropertyName("previous")]
    public string? Previous { get; init; }

    [JsonPropertyName("results")]
    public List<T> Results { get; init; } = [];

    /// <summary>
    /// Gets all items from all pages using the provided fetch function.
    /// </summary>
    public static async Task<List<T>> GetAllPagesAsync(
        Func<int, Task<PaginatedResult<T>>> fetchPage,
        int maxPages = 100,
        CancellationToken cancellationToken = default)
    {
        var allResults = new List<T>();
        var page = 1;

        while (page <= maxPages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await fetchPage(page);
            allResults.AddRange(result.Results);

            if (string.IsNullOrEmpty(result.Next))
                break;

            page++;
        }

        return allResults;
    }
}

/// <summary>
/// Represents bulk operation request parameters.
/// </summary>
public record BulkOperationRequest
{
    [JsonPropertyName("ids")]
    public required int[] Ids { get; init; }

    [JsonPropertyName("dry_run")]
    public bool DryRun { get; init; } = true;

    [JsonPropertyName("confirm")]
    public bool Confirm { get; init; }
}

/// <summary>
/// Represents the result of a bulk operation (dry run or actual).
/// </summary>
public record BulkOperationResult
{
    [JsonPropertyName("affected_ids")]
    public int[] AffectedIds { get; init; } = [];

    [JsonPropertyName("current_values")]
    public Dictionary<int, object>? CurrentValues { get; init; }

    [JsonPropertyName("proposed_changes")]
    public Dictionary<int, object>? ProposedChanges { get; init; }

    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; init; } = [];

    [JsonPropertyName("executed")]
    public bool Executed { get; init; }
}
