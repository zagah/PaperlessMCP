using System.Net;
using System.Text.Json;
using FluentAssertions;
using PaperlessMCP.Tests.Fixtures;
using RichardSzalay.MockHttp;
using PaperlessMCP.Tools;
using Xunit;

namespace PaperlessMCP.Tests.Tools;

public class HealthToolsTests : IDisposable
{
    private readonly MockHttpClientFactory _factory;

    public HealthToolsTests()
    {
        _factory = new MockHttpClientFactory();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task Ping_WhenConnected_ReturnsSuccess()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/")
            .Respond(req =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("X-Version", "2.5.0");
                return response;
            });

        // Act
        var result = await HealthTools.Ping(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("connected").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Ping_WhenConnectionFails_ReturnsError()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/", HttpStatusCode.Unauthorized);

        // Act
        var result = await HealthTools.Ping(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("UPSTREAM_ERROR");
    }

    [Fact]
    public async Task GetCapabilities_ReturnsCapabilitiesInfo()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/")
            .Respond(req =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("X-Version", "2.5.0");
                return response;
            });

        _factory.SetupGet("api/status/", "{}");

        // Act
        var result = await HealthTools.GetCapabilities(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();

        var capabilities = json.RootElement.GetProperty("result");
        capabilities.GetProperty("connected").GetBoolean().Should().BeTrue();
        capabilities.GetProperty("endpoints").Should().NotBeNull();
        capabilities.GetProperty("bulk_edit_methods").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetCapabilities_IncludesAllEndpointCategories()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/")
            .Respond(HttpStatusCode.OK);

        _factory.SetupGet("api/status/", "{}");

        // Act
        var result = await HealthTools.GetCapabilities(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        var endpoints = json.RootElement.GetProperty("result").GetProperty("endpoints");

        endpoints.GetProperty("documents").Should().NotBeNull();
        endpoints.GetProperty("tags").Should().NotBeNull();
        endpoints.GetProperty("correspondents").Should().NotBeNull();
        endpoints.GetProperty("document_types").Should().NotBeNull();
        endpoints.GetProperty("storage_paths").Should().NotBeNull();
        endpoints.GetProperty("custom_fields").Should().NotBeNull();
    }
}
