using System.Net;
using System.Text.Json;
using FluentAssertions;
using PaperlessMCP.Tests.Fixtures;
using RichardSzalay.MockHttp;
using PaperlessMCP.Tools;
using Xunit;

namespace PaperlessMCP.Tests.Tools;

public class CustomFieldToolsTests : IDisposable
{
    private readonly MockHttpClientFactory _factory;

    public CustomFieldToolsTests()
    {
        _factory = new MockHttpClientFactory();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task List_ReturnsCustomFieldList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/custom_fields/*")
            .Respond("application/json", TestFixtures.CustomFields.CreateCustomFieldListJson(4));

        // Act
        var result = await CustomFieldTools.List(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetArrayLength().Should().Be(4);
    }

    [Fact]
    public async Task Get_WhenExists_ReturnsCustomField()
    {
        // Arrange
        _factory.SetupGet("api/custom_fields/1/", TestFixtures.CustomFields.CreateCustomFieldJson(1, "Invoice Number"));

        // Act
        var result = await CustomFieldTools.Get(_factory.Client, 1);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Invoice Number");
    }

    [Fact]
    public async Task Get_WhenNotFound_ReturnsError()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/custom_fields/999/", HttpStatusCode.NotFound);

        // Act
        var result = await CustomFieldTools.Get(_factory.Client, 999);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("NOT_FOUND");
    }

    [Fact]
    public async Task Create_StringField_ReturnsCreatedField()
    {
        // Arrange
        _factory.SetupPost("api/custom_fields/", TestFixtures.CustomFields.CreateCustomFieldJson(1, "Reference Number"));

        // Act
        var result = await CustomFieldTools.Create(_factory.Client, "Reference Number", "string");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Reference Number");
    }

    [Fact]
    public async Task Create_SelectField_IncludesOptions()
    {
        // Arrange
        var selectField = TestFixtures.CustomFields.CreateCustomField(1, "Status", "select");
        _factory.SetupPost("api/custom_fields/", JsonSerializer.Serialize(selectField));

        // Act
        var result = await CustomFieldTools.Create(
            _factory.Client,
            "Status",
            "select",
            selectOptions: "Pending,Approved,Rejected");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Update_WhenSuccessful_ReturnsUpdatedField()
    {
        // Arrange
        _factory.SetupPatch("api/custom_fields/1/", TestFixtures.CustomFields.CreateCustomFieldJson(1, "Updated Field"));

        // Act
        var result = await CustomFieldTools.Update(_factory.Client, 1, name: "Updated Field");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Updated Field");
    }

    [Fact]
    public async Task Delete_WithoutConfirmation_ReturnsDryRun()
    {
        // Arrange
        _factory.SetupGet("api/custom_fields/1/", TestFixtures.CustomFields.CreateCustomFieldJson(1, "To Delete"));

        // Act
        var result = await CustomFieldTools.Delete(_factory.Client, 1, confirm: false);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("CONFIRMATION_REQUIRED");
    }

    [Fact]
    public async Task Delete_WithConfirmation_DeletesField()
    {
        // Arrange
        _factory.SetupDelete("api/custom_fields/1/", HttpStatusCode.NoContent);

        // Act
        var result = await CustomFieldTools.Delete(_factory.Client, 1, confirm: true);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("deleted").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Assign_WhenDocumentExists_AssignsFieldValue()
    {
        // Arrange
        _factory.SetupGet("api/documents/1/", TestFixtures.Documents.CreateDocumentJson(1, "Test Doc"));
        _factory.SetupGet("api/custom_fields/1/", TestFixtures.CustomFields.CreateCustomFieldJson(1, "Invoice Number"));
        _factory.SetupPatch("api/documents/1/", TestFixtures.Documents.CreateDocumentJson(1, "Test Doc"));

        // Act
        var result = await CustomFieldTools.Assign(_factory.Client, documentId: 1, fieldId: 1, value: "INV-001");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("document_id").GetInt32().Should().Be(1);
        json.RootElement.GetProperty("result").GetProperty("field_id").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task Assign_WhenDocumentNotFound_ReturnsError()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/documents/999/", HttpStatusCode.NotFound);

        // Act
        var result = await CustomFieldTools.Assign(_factory.Client, documentId: 999, fieldId: 1, value: "test");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("NOT_FOUND");
    }

    [Fact]
    public async Task Assign_WhenFieldNotFound_ReturnsError()
    {
        // Arrange
        _factory.SetupGet("api/documents/1/", TestFixtures.Documents.CreateDocumentJson(1, "Test Doc"));
        _factory.SetupGetWithStatus("api/custom_fields/999/", HttpStatusCode.NotFound);

        // Act
        var result = await CustomFieldTools.Assign(_factory.Client, documentId: 1, fieldId: 999, value: "test");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("NOT_FOUND");
    }
}
