using System.Net;
using FluentAssertions;
using PaperlessMCP.Client;
using Xunit;

namespace PaperlessMCP.Tests.Client;

public class ApiResultTests
{
    #region Success Tests

    [Fact]
    public void Success_WithValue_ReturnsSuccessResult()
    {
        // Act
        var result = ApiResult<string>.Success("test value");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test value");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_ImplicitBoolConversion_ReturnsTrue()
    {
        // Arrange
        var result = ApiResult<int>.Success(42);

        // Act & Assert
        bool isSuccess = result;
        isSuccess.Should().BeTrue();
    }

    #endregion

    #region Failure Tests

    [Fact]
    public void Failure_WithStatusCodeAndMessage_ReturnsFailureResult()
    {
        // Act
        var result = ApiResult<string>.Failure(HttpStatusCode.NotFound, "Resource not found");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("Resource not found");
        result.Error.ResponseBody.Should().BeNull();
    }

    [Fact]
    public void Failure_WithResponseBody_IncludesBody()
    {
        // Arrange
        var responseBody = """{"error": "Document not found"}""";

        // Act
        var result = ApiResult<string>.Failure(HttpStatusCode.NotFound, "Not found", responseBody);

        // Assert
        result.Error.Should().NotBeNull();
        result.Error!.ResponseBody.Should().Be(responseBody);
    }

    [Fact]
    public void Failure_WithApiError_ReturnsFailureResult()
    {
        // Arrange
        var error = new ApiError(HttpStatusCode.BadRequest, "Invalid input", """{"field": "name is required"}""");

        // Act
        var result = ApiResult<object>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_ImplicitBoolConversion_ReturnsFalse()
    {
        // Arrange
        var result = ApiResult<int>.Failure(HttpStatusCode.InternalServerError, "Server error");

        // Act & Assert
        bool isSuccess = result;
        isSuccess.Should().BeFalse();
    }

    #endregion

    #region ApiError Tests

    [Fact]
    public void ApiError_ToString_FormatsCorrectly()
    {
        // Arrange
        var error = new ApiError(HttpStatusCode.Forbidden, "Access denied", null);

        // Act
        var result = error.ToString();

        // Assert
        result.Should().Be("HTTP 403 Forbidden: Access denied");
    }

    [Fact]
    public void ApiError_ToString_IncludesResponseBody()
    {
        // Arrange
        var error = new ApiError(HttpStatusCode.BadRequest, "Validation failed", """{"name": ["This field is required."]}""");

        // Act
        var result = error.ToString();

        // Assert
        result.Should().Contain("HTTP 400 BadRequest: Validation failed");
        result.Should().Contain("""{"name": ["This field is required."]}""");
    }

    #endregion
}
