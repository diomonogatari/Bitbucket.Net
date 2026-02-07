using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Common.Models;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class ExceptionTests
{
    private static readonly List<Error> SampleErrors =
    [
        new Error { Message = "Test error", Context = "test-context" }
    ];

    #region BitbucketApiException Factory Tests

    [Fact]
    public void Create_400_ReturnsBitbucketBadRequestException()
    {
        var exception = BitbucketApiException.Create(400, SampleErrors, "https://test.com/api");

        Assert.IsType<BitbucketBadRequestException>(exception);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("test-context", exception.Context);
        Assert.Equal("https://test.com/api", exception.RequestUrl);
        Assert.Contains("400", exception.Message);
    }

    [Fact]
    public void Create_401_ReturnsBitbucketAuthenticationException()
    {
        var exception = BitbucketApiException.Create(401, SampleErrors);

        Assert.IsType<BitbucketAuthenticationException>(exception);
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Contains("401", exception.Message);
    }

    [Fact]
    public void Create_403_ReturnsBitbucketForbiddenException()
    {
        var exception = BitbucketApiException.Create(403, SampleErrors);

        Assert.IsType<BitbucketForbiddenException>(exception);
        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Contains("403", exception.Message);
    }

    [Fact]
    public void Create_404_ReturnsBitbucketNotFoundException()
    {
        var exception = BitbucketApiException.Create(404, SampleErrors);

        Assert.IsType<BitbucketNotFoundException>(exception);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    [Fact]
    public void Create_409_ReturnsBitbucketConflictException()
    {
        var exception = BitbucketApiException.Create(409, SampleErrors);

        Assert.IsType<BitbucketConflictException>(exception);
        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Contains("409", exception.Message);
    }

    [Fact]
    public void Create_422_ReturnsBitbucketValidationException()
    {
        var exception = BitbucketApiException.Create(422, SampleErrors);

        Assert.IsType<BitbucketValidationException>(exception);
        Assert.Equal((HttpStatusCode)422, exception.StatusCode);
        Assert.Contains("422", exception.Message);
    }

    [Fact]
    public void Create_429_ReturnsBitbucketRateLimitException()
    {
        var exception = BitbucketApiException.Create(429, SampleErrors);

        Assert.IsType<BitbucketRateLimitException>(exception);
        Assert.Equal((HttpStatusCode)429, exception.StatusCode);
        Assert.Contains("429", exception.Message);
    }

    [Theory]
    [InlineData(500)]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public void Create_5xx_ReturnsBitbucketServerException(int statusCode)
    {
        var exception = BitbucketApiException.Create(statusCode, SampleErrors);

        Assert.IsType<BitbucketServerException>(exception);
        Assert.Equal((HttpStatusCode)statusCode, exception.StatusCode);
        Assert.Contains(statusCode.ToString(), exception.Message);
    }

    [Theory]
    [InlineData(418)]
    [InlineData(451)]
    public void Create_OtherStatus_ReturnsBitbucketApiException(int statusCode)
    {
        var exception = BitbucketApiException.Create(statusCode, SampleErrors);

        Assert.IsType<BitbucketApiException>(exception);
        Assert.IsNotType<BitbucketBadRequestException>(exception);
        Assert.IsNotType<BitbucketServerException>(exception);
        Assert.Equal((HttpStatusCode)statusCode, exception.StatusCode);
    }

    #endregion

    #region Error Message Building Tests

    [Fact]
    public void Create_WithNoErrors_BuildsGenericMessage()
    {
        var exception = BitbucketApiException.Create(400, new List<Error>());

        Assert.Contains("400", exception.Message);
        Assert.Contains("BadRequest", exception.Message);
    }

    [Fact]
    public void Create_WithNullErrors_BuildsGenericMessage()
    {
        var exception = BitbucketApiException.Create(400, null!);

        Assert.Contains("400", exception.Message);
    }

    [Fact]
    public void Create_WithContextInError_IncludesContextInMessage()
    {
        var errors = new List<Error>
        {
            new() { Message = "Field is invalid", Context = "username" }
        };
        var exception = BitbucketApiException.Create(400, errors);

        Assert.Contains("[username]", exception.Message);
        Assert.Contains("Field is invalid", exception.Message);
    }

    [Fact]
    public void Create_WithoutContextInError_OmitsContextFromMessage()
    {
        var errors = new List<Error>
        {
            new() { Message = "Something went wrong", Context = null }
        };
        var exception = BitbucketApiException.Create(400, errors);

        Assert.DoesNotContain("[", exception.Message.Replace("400", "")
            .Replace("BadRequest", "")
            .Replace("[", "X"));
        Assert.Contains("Something went wrong", exception.Message);
    }

    [Fact]
    public void Create_WithMultipleErrors_IncludesAllMessages()
    {
        var errors = new List<Error>
        {
            new() { Message = "Error 1", Context = "field1" },
            new() { Message = "Error 2", Context = "field2" }
        };
        var exception = BitbucketApiException.Create(400, errors);

        Assert.Contains("Error 1", exception.Message);
        Assert.Contains("Error 2", exception.Message);
        Assert.Contains("[field1]", exception.Message);
        Assert.Contains("[field2]", exception.Message);
    }

    #endregion

    #region Exception Properties Tests

    [Fact]
    public void BitbucketApiException_Properties_AreSetCorrectly()
    {
        var errors = new List<Error>
        {
            new() { Message = "Test", Context = "ctx" }
        };
        var exception = new BitbucketApiException("Test message", HttpStatusCode.NotFound, errors, "https://api.test");

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("ctx", exception.Context);
        Assert.Equal("https://api.test", exception.RequestUrl);
        Assert.Single(exception.Errors);
        Assert.Equal("Test message", exception.Message);
    }

    [Fact]
    public void BitbucketApiException_WithNullErrors_SetsEmptyCollection()
    {
        var exception = new BitbucketApiException("Test", HttpStatusCode.NotFound, null!);

        Assert.NotNull(exception.Errors);
        Assert.Empty(exception.Errors);
        Assert.Null(exception.Context);
    }

    [Fact]
    public void BitbucketApiException_WithEmptyErrors_SetsContextToNull()
    {
        var exception = new BitbucketApiException("Test", HttpStatusCode.NotFound, new List<Error>());

        Assert.Null(exception.Context);
    }

    #endregion

    #region Inner Exception Tests

    [Fact]
    public void BitbucketApiException_WithInnerException_PreservesInnerException()
    {
        var innerException = new System.Exception("Inner");
        var exception = new BitbucketApiException(
            "Outer",
            HttpStatusCode.InternalServerError,
            SampleErrors,
            innerException,
            "https://test.com");

        Assert.Equal(innerException, exception.InnerException);
        Assert.Equal("Outer", exception.Message);
    }

    #endregion
}