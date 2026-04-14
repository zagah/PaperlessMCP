using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using ModelContextProtocol.Server;
using Xunit;

namespace PaperlessMCP.Tests.Tools;

public class ToolNamingTests
{
    private static readonly Regex AnthropicToolNamePattern = new("^[a-zA-Z0-9_-]{1,64}$");
    private static readonly Regex HyphenConventionPattern = new("^paperless-[a-z][a-z0-9-]*$");

    private static List<string> GetAllToolNames()
    {
        var toolAssembly = typeof(PaperlessMCP.Tools.HealthTools).Assembly;

        return toolAssembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            .SelectMany(m => m.GetCustomAttributes<McpServerToolAttribute>())
            .Where(a => a.Name is not null)
            .Select(a => a.Name!)
            .ToList();
    }

    [Fact]
    public void AllToolNames_ShouldMatchAnthropicApiNamingRules()
    {
        var toolNames = GetAllToolNames();
        toolNames.Should().NotBeEmpty("expected to find at least one McpServerTool attribute");

        var violations = toolNames
            .Where(name => !AnthropicToolNamePattern.IsMatch(name))
            .ToList();

        violations.Should().BeEmpty(
            "tool names must match ^[a-zA-Z0-9_-]{{1,64}}$ per Anthropic API rules, " +
            $"but found: {string.Join(", ", violations)}");
    }

    [Fact]
    public void AllToolNames_ShouldNotContainDots()
    {
        var toolNames = GetAllToolNames();

        var dotNames = toolNames.Where(name => name.Contains('.')).ToList();

        dotNames.Should().BeEmpty(
            $"dots are not allowed in tool names, but found: {string.Join(", ", dotNames)}");
    }

    [Fact]
    public void AllToolNames_ShouldFollowHyphenConvention()
    {
        var toolNames = GetAllToolNames();
        toolNames.Should().NotBeEmpty("expected to find at least one McpServerTool attribute");

        var violations = toolNames
            .Where(name => !HyphenConventionPattern.IsMatch(name))
            .ToList();

        violations.Should().BeEmpty(
            "all tool names must follow the all-hyphens convention matching ^paperless-[a-z][a-z0-9-]*$, " +
            $"but found: {string.Join(", ", violations)}");
    }
}
