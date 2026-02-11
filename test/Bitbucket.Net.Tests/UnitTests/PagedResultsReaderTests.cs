using Bitbucket.Net.Common;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class PagedResultsReaderTests
{
    [Fact]
    public void ReadMetadata_FullPayload_ExtractsAllFields()
    {
        var json = """{"size":25,"limit":25,"isLastPage":false,"start":0,"nextPageStart":25,"values":[{"id":1},{"id":2}]}"""u8;
        var metadata = PagedResultsReader.ReadMetadata(json);

        Assert.False(metadata.IsLastPage);
        Assert.Equal(25, metadata.NextPageStart);
        Assert.Equal(0, metadata.Start);
        Assert.Equal(25, metadata.Limit);
        Assert.Equal(25, metadata.Size);
    }

    [Fact]
    public void ReadMetadata_LastPage_ReturnsTrue()
    {
        var json = """{"size":10,"limit":25,"isLastPage":true,"start":0,"values":[]}"""u8;
        var metadata = PagedResultsReader.ReadMetadata(json);

        Assert.True(metadata.IsLastPage);
        Assert.Null(metadata.NextPageStart);
        Assert.Equal(10, metadata.Size);
    }

    [Fact]
    public void ReadMetadata_EmptyValues_Works()
    {
        var json = """{"size":0,"limit":25,"isLastPage":true,"values":[]}"""u8;
        var metadata = PagedResultsReader.ReadMetadata(json);

        Assert.True(metadata.IsLastPage);
        Assert.Equal(0, metadata.Size);
    }

    [Fact]
    public void ReadMetadata_NestedValues_SkipsCorrectly()
    {
        var json = """{"size":2,"limit":25,"isLastPage":false,"nextPageStart":25,"values":[{"id":1,"title":"Test","nested":{"deep":true}},{"id":2,"title":"Another","tags":["a","b"]}]}"""u8;
        var metadata = PagedResultsReader.ReadMetadata(json);

        Assert.False(metadata.IsLastPage);
        Assert.Equal(25, metadata.NextPageStart);
        Assert.Equal(2, metadata.Size);
    }

    [Fact]
    public void ReadMetadata_MissingOptionalFields_DefaultsCorrectly()
    {
        var json = """{"isLastPage":true,"values":[]}"""u8;
        var metadata = PagedResultsReader.ReadMetadata(json);

        Assert.True(metadata.IsLastPage);
        Assert.Null(metadata.NextPageStart);
        Assert.Null(metadata.Start);
        Assert.Null(metadata.Limit);
        Assert.Equal(0, metadata.Size);
    }
}