namespace PaperlessMCP.Utils;

/// <summary>
/// Shared parsing utilities for MCP tool parameters.
/// </summary>
public static class ParsingHelpers
{
    /// <summary>
    /// Parses a comma-separated string of integers into an array.
    /// </summary>
    /// <param name="input">Comma-separated integer values (e.g., "1,2,3")</param>
    /// <returns>Array of parsed integers, or null if input is empty/whitespace</returns>
    public static int[]? ParseIntArray(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
            .Where(n => n.HasValue)
            .Select(n => n!.Value)
            .ToArray();
    }

    /// <summary>
    /// Parses a date string into a DateTime.
    /// </summary>
    /// <param name="input">Date string in any standard format</param>
    /// <returns>Parsed DateTime, or null if input is empty/invalid</returns>
    public static DateTime? ParseDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        return DateTime.TryParse(input, out var date) ? date : null;
    }
}
