using System.Net;
using System.Text.Json;
using FluentAssertions;
using PaperlessMCP.Tests.Fixtures;
using RichardSzalay.MockHttp;
using PaperlessMCP.Tools;
using Xunit;

namespace PaperlessMCP.Tests.Tools;

public class StoragePathToolsTests : IDisposable
{
    private readonly MockHttpClientFactory _factory;

    public StoragePathToolsTests()
    {
        _factory = new MockHttpClientFactory();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task List_ReturnsStoragePathList()
    {
        // Arrange
        _factory.MockHandler
            .When(HttpMethod.Get, "https://paperless.example.com/api/storage_paths/*")
            .Respond("application/json", TestFixtures.StoragePaths.CreateStoragePathListJson(3));

        // Act
        var result = await StoragePathTools.List(_factory.Client);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetArrayLength().Should().Be(3);
    }

    [Fact]
    public async Task Get_WhenExists_ReturnsStoragePath()
    {
        // Arrange
        _factory.SetupGet("api/storage_paths/1/", TestFixtures.StoragePaths.CreateStoragePathJson(1, "Archive"));

        // Act
        var result = await StoragePathTools.Get(_factory.Client, 1);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Archive");
    }

    [Fact]
    public async Task Get_WhenNotFound_ReturnsError()
    {
        // Arrange
        _factory.SetupGetWithStatus("api/storage_paths/999/", HttpStatusCode.NotFound);

        // Act
        var result = await StoragePathTools.Get(_factory.Client, 999);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("NOT_FOUND");
    }

    [Fact]
    public async Task Create_WhenSuccessful_ReturnsCreatedPath()
    {
        // Arrange
        _factory.SetupPost("api/storage_paths/", TestFixtures.StoragePaths.CreateStoragePathJson(1, "New Archive"));

        // Act
        var result = await StoragePathTools.Create(_factory.Client, "New Archive", "{correspondent}/{year}");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("New Archive");
    }

    [Fact]
    public async Task Update_WhenSuccessful_ReturnsUpdatedPath()
    {
        // Arrange
        _factory.SetupPatch("api/storage_paths/1/", TestFixtures.StoragePaths.CreateStoragePathJson(1, "Updated Path"));

        // Act
        var result = await StoragePathTools.Update(_factory.Client, 1, name: "Updated Path");

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("name").GetString().Should().Be("Updated Path");
    }

    [Fact]
    public async Task Delete_WithoutConfirmation_ReturnsDryRun()
    {
        // Arrange
        _factory.SetupGet("api/storage_paths/1/", TestFixtures.StoragePaths.CreateStoragePathJson(1, "To Delete"));

        // Act
        var result = await StoragePathTools.Delete(_factory.Client, 1, confirm: false);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeFalse();
        json.RootElement.GetProperty("error").GetProperty("code").GetString().Should().Be("CONFIRMATION_REQUIRED");
    }

    [Fact]
    public async Task Delete_WithConfirmation_DeletesPath()
    {
        // Arrange
        _factory.SetupDelete("api/storage_paths/1/", HttpStatusCode.NoContent);

        // Act
        var result = await StoragePathTools.Delete(_factory.Client, 1, confirm: true);

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
        var result = await StoragePathTools.BulkDelete(_factory.Client, "1,2", dryRun: false, confirm: true);

        // Assert
        var json = JsonDocument.Parse(result);
        json.RootElement.GetProperty("ok").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("result").GetProperty("executed").GetBoolean().Should().BeTrue();
    }
}
