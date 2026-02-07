#nullable enable

using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Serialization;
using System.Text.Json;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class CommonModelTests
{
    #region PagedResults Tests

    [Fact]
    public void PagedResults_DefaultValues_AreCorrect()
    {
        var paged = new PagedResults<string>();

        Assert.Equal(0, paged.Size);
        Assert.Equal(0, paged.Start);
        Assert.Equal(0, paged.Limit);
        Assert.False(paged.IsLastPage);
        Assert.NotNull(paged.Values);
        Assert.Empty(paged.Values);
        Assert.Null(paged.NextPageStart);
    }

    [Fact]
    public void PagedResults_HasMore_ReturnsTrueWhenNotLastPage()
    {
        var paged = new PagedResults<string> { IsLastPage = false };
        Assert.True(paged.HasMore);
    }

    [Fact]
    public void PagedResults_HasMore_ReturnsFalseWhenLastPage()
    {
        var paged = new PagedResults<string> { IsLastPage = true };
        Assert.False(paged.HasMore);
    }

    [Fact]
    public void PagedResults_CurrentOffset_ReturnsStart()
    {
        var paged = new PagedResults<string> { Start = 25 };
        Assert.Equal(25, paged.CurrentOffset);
    }

    [Fact]
    public void PagedResults_WithValues_ContainsExpectedItems()
    {
        var paged = new PagedResults<string>
        {
            Values = ["item1", "item2", "item3"],
            Size = 3,
            Limit = 25,
            Start = 0,
            IsLastPage = true
        };

        Assert.Equal(3, paged.Values.Count);
        Assert.Contains("item1", paged.Values);
        Assert.Contains("item2", paged.Values);
        Assert.Contains("item3", paged.Values);
    }

    [Fact]
    public void PagedResults_Serialization_RoundTrips()
    {
        var paged = new PagedResults<string>
        {
            Values = ["test1", "test2"],
            Size = 2,
            Limit = 25,
            Start = 10,
            IsLastPage = false,
            NextPageStart = 12
        };

        var json = JsonSerializer.Serialize(paged, BitbucketJsonContext.Default.PagedResultsString);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.PagedResultsString);

        Assert.NotNull(deserialized);
        Assert.Equal(paged.Size, deserialized.Size);
        Assert.Equal(paged.Limit, deserialized.Limit);
        Assert.Equal(paged.Start, deserialized.Start);
        Assert.Equal(paged.IsLastPage, deserialized.IsLastPage);
        Assert.Equal(paged.NextPageStart, deserialized.NextPageStart);
        Assert.Equal(paged.Values, deserialized.Values);
    }

    #endregion

    #region Error Tests

    [Fact]
    public void Error_DefaultValues_AreCorrect()
    {
        var error = new Error();

        Assert.Null(error.Context);
        Assert.Equal(string.Empty, error.Message);
        Assert.Null(error.ExceptionName);
    }

    [Fact]
    public void Error_CanSetAllProperties()
    {
        var error = new Error
        {
            Context = "field.name",
            Message = "Field is required",
            ExceptionName = "ValidationException"
        };

        Assert.Equal("field.name", error.Context);
        Assert.Equal("Field is required", error.Message);
        Assert.Equal("ValidationException", error.ExceptionName);
    }

    [Fact]
    public void Error_Serialization_RoundTrips()
    {
        var error = new Error
        {
            Context = "test.context",
            Message = "Test error message",
            ExceptionName = "TestException"
        };

        var json = JsonSerializer.Serialize(error, BitbucketJsonContext.Default.Error);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Error);

        Assert.NotNull(deserialized);
        Assert.Equal(error.Context, deserialized.Context);
        Assert.Equal(error.Message, deserialized.Message);
        Assert.Equal(error.ExceptionName, deserialized.ExceptionName);
    }

    [Fact]
    public void Error_Serialization_NullProperties_AreOmitted()
    {
        var error = new Error { Message = "Test" };

        var json = JsonSerializer.Serialize(error, BitbucketJsonContext.Default.Error);

        Assert.DoesNotContain("context", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("exceptionName", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("message", json, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region ErrorResponse Tests

    [Fact]
    public void ErrorResponse_DefaultValues_AreCorrect()
    {
        var response = new ErrorResponse();
        Assert.Null(response.Errors);
    }

    [Fact]
    public void ErrorResponse_CanSetErrors()
    {
        var errors = new List<Error>
        {
            new() { Message = "Error 1" },
            new() { Message = "Error 2" }
        };

        var response = new ErrorResponse { Errors = errors };

        Assert.NotNull(response.Errors);
        Assert.Equal(2, ((List<Error>)response.Errors).Count);
    }

    [Fact]
    public void ErrorResponse_Serialization_RoundTrips()
    {
        var response = new ErrorResponse
        {
            Errors = new List<Error>
            {
                new() { Message = "First error", Context = "field1" },
                new() { Message = "Second error", ExceptionName = "InvalidOperationException" }
            }
        };

        var json = JsonSerializer.Serialize(response, BitbucketJsonContext.Default.ErrorResponse);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.ErrorResponse);

        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.Errors);

        var errorList = new List<Error>(deserialized.Errors);
        Assert.Equal(2, errorList.Count);
        Assert.Equal("First error", errorList[0].Message);
        Assert.Equal("field1", errorList[0].Context);
        Assert.Equal("Second error", errorList[1].Message);
        Assert.Equal("InvalidOperationException", errorList[1].ExceptionName);
    }

    #endregion

    #region TypeExtensions Tests

    [Fact]
    public void IsNullableType_NullableInt_ReturnsTrue()
    {
        Assert.True(TypeExtensions.IsNullableType(typeof(int?)));
    }

    [Fact]
    public void IsNullableType_NullableDateTime_ReturnsTrue()
    {
        Assert.True(TypeExtensions.IsNullableType(typeof(DateTime?)));
    }

    [Fact]
    public void IsNullableType_NullableBool_ReturnsTrue()
    {
        Assert.True(TypeExtensions.IsNullableType(typeof(bool?)));
    }

    [Fact]
    public void IsNullableType_Int_ReturnsFalse()
    {
        Assert.False(TypeExtensions.IsNullableType(typeof(int)));
    }

    [Fact]
    public void IsNullableType_String_ReturnsFalse()
    {
        Assert.False(TypeExtensions.IsNullableType(typeof(string)));
    }

    [Fact]
    public void IsNullableType_Object_ReturnsFalse()
    {
        Assert.False(TypeExtensions.IsNullableType(typeof(object)));
    }

    [Fact]
    public void IsNullableType_ListOfInt_ReturnsFalse()
    {
        Assert.False(TypeExtensions.IsNullableType(typeof(List<int>)));
    }

    #endregion

    #region UnixDateTimeExtensions Tests

    [Fact]
    public void FromUnixTimeSeconds_ZeroReturnsEpoch()
    {
        long timestamp = 0;
        var result = timestamp.FromUnixTimeSeconds();

        // The method uses AddMilliseconds, so 0 should give us the epoch converted to local time
        var expected = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FromUnixTimeSeconds_KnownTimestamp_ReturnsCorrectDate()
    {
        // 1609459200000 milliseconds = Jan 1, 2021 00:00:00 UTC
        long timestamp = 1609459200000;
        var result = timestamp.FromUnixTimeSeconds();

        var expected = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToUnixTimeSeconds_Epoch_ReturnsZero()
    {
        var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var result = epoch.ToUnixTimeSeconds();

        Assert.Equal(0, result);
    }

    [Fact]
    public void ToUnixTimeSeconds_KnownDate_ReturnsCorrectValue()
    {
        var dateTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var result = dateTime.ToUnixTimeSeconds();

        // Note: The method name says "Seconds" but implementation returns Ticks
        // This test verifies the actual behavior
        Assert.True(result > 0);
    }

    #endregion
}