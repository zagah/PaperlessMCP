using System.Net;
using FluentAssertions;
using PaperlessMCP.Models.Correspondents;
using RichardSzalay.MockHttp;
using Xunit;
using PaperlessMCP.Models.CustomFields;
using PaperlessMCP.Models.DocumentTypes;
using PaperlessMCP.Models.StoragePaths;
using PaperlessMCP.Models.Tags;
using PaperlessMCP.Tests.Fixtures;

namespace PaperlessMCP.Tests.Client;

public class PaperlessClientTests : IDisposable
{
    private readonly MockHttpClientFactory _factory;

    public PaperlessClientTests()
    {
        _factory = new MockHttpClientFactory();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    #region Ping Tests

    [Fact]
    public async Task PingAsync_WhenSuccessful_ReturnsSuccess()
    {
        // Arrange
        _factory.SetupGet("api/status/", """{"pngx_version": "2.0.0", "server_os": "Linux"}""");

        // Act
        var (success, version, error) = await _factory.Client.PingAsync();

        // Assert
        success.Should().BeTrue();
        version.Should().Be("2.0.0");
        error.Should().BeNull();
    }

    [Fact]
    public async Task PingAsync_WhenUnauthorized_ReturnsFailure()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/status/", HttpStatusCode.Unauthorized);

        // Act
        var (success, version, error) = await _factory.Client.PingAsync();

        // Assert
        success.Should().BeFalse();
        version.Should().BeNull();
        error.Should().Contain("401");
    }

    #endregion

    #region Document Tests

    [Fact]
    public async Task SearchDocumentsAsync_WithQuery_ReturnsResults()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/documents/*")
            .Respond("application/json", TestFixtures.Documents.CreateSearchResultsJson(5));

        // Act
        var result = await _factory.Client.SearchDocumentsAsync(query: "test");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
        result.Results.Should().HaveCount(5);
    }

    [Fact]
    public async Task SearchDocumentsAsync_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/documents/*")
            .Respond("application/json", TestFixtures.Documents.CreateSearchResultsJson(2));

        // Act
        var result = await _factory.Client.SearchDocumentsAsync(
            query: "invoice",
            tags: [1, 2],
            correspondent: 3,
            documentType: 4);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetDocumentAsync_WhenExists_ReturnsDocument()
    {
        // Arrange
        _factory.SetupGet("api/documents/1/", TestFixtures.Documents.CreateDocumentJson(1, "My Document"));

        // Act
        var result = await _factory.Client.GetDocumentAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("My Document");
    }

    [Fact]
    public async Task GetDocumentAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/documents/999/", HttpStatusCode.NotFound);

        // Act
        var result = await _factory.Client.GetDocumentAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteDocumentAsync_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        _factory.SetupDelete("api/documents/1/", HttpStatusCode.NoContent);

        // Act
        var result = await _factory.Client.DeleteDocumentAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDocumentAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _factory.SetupDelete("api/documents/999/", HttpStatusCode.NotFound);

        // Act
        var result = await _factory.Client.DeleteDocumentAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetDocumentDownloadInfo_ReturnsCorrectUrls()
    {
        // Act
        var result = _factory.Client.GetDocumentDownloadInfo(1, "Test Doc", "test.pdf");

        // Assert
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Doc");
        result.OriginalFileName.Should().Be("test.pdf");
        result.DownloadUrl.Should().Be("https://paperless.example.com/api/documents/1/download/");
        result.PreviewUrl.Should().Be("https://paperless.example.com/api/documents/1/preview/");
        result.ThumbnailUrl.Should().Be("https://paperless.example.com/api/documents/1/thumb/");
    }

    #endregion

    #region Tag Tests

    [Fact]
    public async Task GetTagsAsync_ReturnsTagList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/tags/*")
            .Respond("application/json", TestFixtures.Tags.CreateTagListJson(5));

        // Act
        var result = await _factory.Client.GetTagsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
        result.Results.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTagAsync_WhenExists_ReturnsTag()
    {
        // Arrange
        _factory.SetupGet("api/tags/1/", TestFixtures.Tags.CreateTagJson(1, "Important"));

        // Act
        var result = await _factory.Client.GetTagAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Important");
    }

    [Fact]
    public async Task CreateTagAsync_WhenSuccessful_ReturnsTag()
    {
        // Arrange
        _factory.SetupPost("api/tags/", TestFixtures.Tags.CreateTagJson(5, "New Tag"));

        // Act
        var result = await _factory.Client.CreateTagAsync(new TagCreateRequest { Name = "New Tag" });

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(5);
        result.Name.Should().Be("New Tag");
    }

    [Fact]
    public async Task DeleteTagAsync_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        _factory.SetupDelete("api/tags/1/", HttpStatusCode.NoContent);

        // Act
        var result = await _factory.Client.DeleteTagAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Correspondent Tests

    [Fact]
    public async Task GetCorrespondentsAsync_ReturnsCorrespondentList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/correspondents/*")
            .Respond("application/json", TestFixtures.Correspondents.CreateCorrespondentListJson(3));

        // Act
        var result = await _factory.Client.GetCorrespondentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task CreateCorrespondentAsync_WhenSuccessful_ReturnsCorrespondent()
    {
        // Arrange
        _factory.SetupPost("api/correspondents/", TestFixtures.Correspondents.CreateCorrespondentJson(1, "ACME Corp"));

        // Act
        var result = await _factory.Client.CreateCorrespondentAsync(new CorrespondentCreateRequest { Name = "ACME Corp" });

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ACME Corp");
    }

    #endregion

    #region Document Type Tests

    [Fact]
    public async Task GetDocumentTypesAsync_ReturnsDocumentTypeList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/document_types/*")
            .Respond("application/json", TestFixtures.DocumentTypes.CreateDocumentTypeListJson(4));

        // Act
        var result = await _factory.Client.GetDocumentTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(4);
    }

    [Fact]
    public async Task CreateDocumentTypeAsync_WhenSuccessful_ReturnsDocumentType()
    {
        // Arrange
        _factory.SetupPost("api/document_types/", TestFixtures.DocumentTypes.CreateDocumentTypeJson(1, "Invoice"));

        // Act
        var result = await _factory.Client.CreateDocumentTypeAsync(new DocumentTypeCreateRequest { Name = "Invoice" });

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Invoice");
    }

    #endregion

    #region Storage Path Tests

    [Fact]
    public async Task GetStoragePathsAsync_ReturnsStoragePathList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/storage_paths/*")
            .Respond("application/json", TestFixtures.StoragePaths.CreateStoragePathListJson(2));

        // Act
        var result = await _factory.Client.GetStoragePathsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task CreateStoragePathAsync_WhenSuccessful_ReturnsStoragePath()
    {
        // Arrange
        _factory.SetupPost("api/storage_paths/", TestFixtures.StoragePaths.CreateStoragePathJson(1, "Archive"));

        // Act
        var result = await _factory.Client.CreateStoragePathAsync(new StoragePathCreateRequest
        {
            Name = "Archive",
            Path = "{correspondent}/{year}"
        });

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Archive");
    }

    #endregion

    #region Custom Field Tests

    [Fact]
    public async Task GetCustomFieldsAsync_ReturnsCustomFieldList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/custom_fields/*")
            .Respond("application/json", TestFixtures.CustomFields.CreateCustomFieldListJson(3));

        // Act
        var result = await _factory.Client.GetCustomFieldsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task CreateCustomFieldAsync_WhenSuccessful_ReturnsCustomField()
    {
        // Arrange
        _factory.SetupPost("api/custom_fields/", TestFixtures.CustomFields.CreateCustomFieldJson(1, "Invoice Number"));

        // Act
        var result = await _factory.Client.CreateCustomFieldAsync(new CustomFieldCreateRequest
        {
            Name = "Invoice Number",
            DataType = "string"
        });

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Invoice Number");
    }

    #endregion

    #region Bulk Operations Tests

    [Fact]
    public async Task BulkEditDocumentsAsync_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        _factory.SetupPost("api/documents/bulk_edit/", "{}");

        // Act
        var result = await _factory.Client.BulkEditDocumentsAsync([1, 2, 3], "add_tag", new { tag = 5 });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task BulkEditObjectsAsync_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        _factory.SetupPost("api/bulk_edit_objects/", "{}");

        // Act
        var result = await _factory.Client.BulkEditObjectsAsync([1, 2], "tags", "delete");

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
