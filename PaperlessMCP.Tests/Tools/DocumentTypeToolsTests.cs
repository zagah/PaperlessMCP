using System.Net;
using System.Text.Json;
using FluentAssertions;
using PaperlessMCP.Tests.Fixtures;
using RichardSzalay.MockHttp;
using PaperlessMCP.Tools;
using Xunit;

namespace PaperlessMCP.Tests.Tools;

public class DocumentTypeToolsTests : IDisposable
{
    private readonly MockHttpClientFactory _factory;

    public DocumentTypeToolsTests()
    {
        _factory = new MockHttpClientFactory();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task List_ReturnsDocumentTypeList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/document_types/*")
            .Respond("application/json", TestFixtures.DocumentTypes.CreateDocumentTypeListJson(4));

        // Act
        var result = await DocumentTypeTools.List(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetArrayLength().Should().Be(4);
    }

    [Fact]
    public async Task Get_WhenExists_ReturnsDocumentType()
    {
        // Arrange
        _factory.SetupGet("api/document_types/1/", TestFixtures.DocumentTypes.CreateDocumentTypeJson(1, "Invoice"));

        // Act
        var result = await DocumentTypeTools.Get(_factory.Client, 1);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Invoice");
    }

    [Fact]
    public async Task Get_WhenNotFound_ReturnsError()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/document_types/999/", HttpStatusCode.NotFound);

        // Act
        var result = await DocumentTypeTools.Get(_factory.Client, 999);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("NOT_FOUND");
    }

    [Fact]
    public async Task Create_WhenSuccessful_ReturnsCreatedType()
    {
        // Arrange
        _factory.SetupPost("api/document_types/", TestFixtures.DocumentTypes.CreateDocumentTypeJson(1, "Contract"));

        // Act
        var result = await DocumentTypeTools.Create(_factory.Client, "Contract");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Contract");
    }

    [Fact]
    public async Task Update_WhenSuccessful_ReturnsUpdatedType()
    {
        // Arrange
        _factory.SetupPatch("api/document_types/1/", TestFixtures.DocumentTypes.CreateDocumentTypeJson(1, "Updated Type"));

        // Act
        var result = await DocumentTypeTools.Update(_factory.Client, 1, name: "Updated Type");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Updated Type");
    }

    [Fact]
    public async Task Delete_WithoutConfirmation_ReturnsDryRun()
    {
        // Arrange
        _factory.SetupGet("api/document_types/1/", TestFixtures.DocumentTypes.CreateDocumentTypeJson(1, "To Delete"));

        // Act
        var result = await DocumentTypeTools.Delete(_factory.Client, 1, confirm: false);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("CONFIRMATION_REQUIRED");
    }

    [Fact]
    public async Task Delete_WithConfirmation_DeletesType()
    {
        // Arrange
        _factory.SetupDelete("api/document_types/1/", HttpStatusCode.NoContent);

        // Act
        var result = await DocumentTypeTools.Delete(_factory.Client, 1, confirm: true);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("deleted").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task BulkDelete_WithConfirmation_ExecutesDeletion()
    {
        // Arrange
        _factory.SetupPost("api/bulk_edit_objects/", "{}");

        // Act
        var result = await DocumentTypeTools.BulkDelete(_factory.Client, "1,2,3", dryRun: false, confirm: true);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("executed").GetBoolean().Should().BeTrue();
    }
}
