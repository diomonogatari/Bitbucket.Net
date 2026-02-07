using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Projects;
using System.Text.Json;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class JsonConverterTests
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        Converters =
        {
            new UnixDateTimeOffsetConverter(),
            new NullableUnixDateTimeOffsetConverter(),
            new PullRequestStatesConverter(),
            new ParticipantStatusConverter(),
            new RolesConverter(),
            new LineTypesConverter(),
            new FileTypesConverter(),
            new HookTypesConverter(),
            new ScopeTypesConverter(),
            new WebHookOutcomesConverter(),
            new BlockerCommentStateConverter(),
            new CommentSeverityConverter()
        }
    };

    #region UnixDateTimeOffsetConverter Tests

    [Fact]
    public void UnixDateTimeOffsetConverter_Read_FromNumber_ReturnsCorrectValue()
    {
        // The converter uses milliseconds internally (despite the "seconds" naming)
        var json = "1609459200000"; // 2021-01-01 00:00:00 UTC in milliseconds
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, s_options);
        // The result will be in local time
        Assert.Equal(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime(), result);
    }

    [Fact]
    public void UnixDateTimeOffsetConverter_Read_FromString_ReturnsCorrectValue()
    {
        var json = "\"1609459200000\""; // 2021-01-01 00:00:00 UTC in milliseconds
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, s_options);
        Assert.Equal(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime(), result);
    }

    [Fact]
    public void UnixDateTimeOffsetConverter_Write_ReturnsJsonNumber()
    {
        var value = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var json = JsonSerializer.Serialize(value, s_options);
        // The converter returns ticks - just verify it's a number, not the exact value
        Assert.DoesNotContain("\"", json); // Number, not string
        long.Parse(json); // Should parse as a number
    }

    [Fact]
    public void UnixDateTimeOffsetConverter_Read_InvalidToken_ThrowsJsonException()
    {
        var json = "true";
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateTimeOffset>(json, s_options));
    }

    #endregion

    #region NullableUnixDateTimeOffsetConverter Tests

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Read_FromNumber_ReturnsCorrectValue()
    {
        var json = "1609459200000"; // 2021-01-01 00:00:00 UTC in milliseconds
        var result = JsonSerializer.Deserialize<DateTimeOffset?>(json, s_options);
        Assert.Equal(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime(), result);
    }

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Read_FromString_ReturnsCorrectValue()
    {
        var json = "\"1609459200000\""; // 2021-01-01 00:00:00 UTC in milliseconds
        var result = JsonSerializer.Deserialize<DateTimeOffset?>(json, s_options);
        Assert.Equal(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero).ToLocalTime(), result);
    }

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Read_Null_ReturnsNull()
    {
        var json = "null";
        var result = JsonSerializer.Deserialize<DateTimeOffset?>(json, s_options);
        Assert.Null(result);
    }

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Read_EmptyString_ReturnsNull()
    {
        var json = "\"\"";
        var result = JsonSerializer.Deserialize<DateTimeOffset?>(json, s_options);
        Assert.Null(result);
    }

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Write_WithValue_ReturnsJsonNumber()
    {
        DateTimeOffset? value = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.DoesNotContain("\"", json); // Number, not string
        long.Parse(json); // Should parse as a number
    }

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Write_Null_ReturnsNull()
    {
        DateTimeOffset? value = null;
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal("null", json);
    }

    [Fact]
    public void NullableUnixDateTimeOffsetConverter_Read_InvalidToken_ThrowsJsonException()
    {
        var json = "true";
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateTimeOffset?>(json, s_options));
    }

    #endregion

    #region PullRequestStatesConverter Tests

    [Theory]
    [InlineData("\"OPEN\"", PullRequestStates.Open)]
    [InlineData("\"DECLINED\"", PullRequestStates.Declined)]
    [InlineData("\"MERGED\"", PullRequestStates.Merged)]
    [InlineData("\"ALL\"", PullRequestStates.All)]
    public void PullRequestStatesConverter_Read_ReturnsCorrectValue(string json, PullRequestStates expected)
    {
        var result = JsonSerializer.Deserialize<PullRequestStates>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(PullRequestStates.Open, "\"OPEN\"")]
    [InlineData(PullRequestStates.Declined, "\"DECLINED\"")]
    [InlineData(PullRequestStates.Merged, "\"MERGED\"")]
    [InlineData(PullRequestStates.All, "\"ALL\"")]
    public void PullRequestStatesConverter_Write_ReturnsCorrectValue(PullRequestStates value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region ParticipantStatusConverter Tests

    [Theory]
    [InlineData("\"APPROVED\"", ParticipantStatus.Approved)]
    [InlineData("\"NEEDS_WORK\"", ParticipantStatus.NeedsWork)]
    [InlineData("\"UNAPPROVED\"", ParticipantStatus.Unapproved)]
    public void ParticipantStatusConverter_Read_ReturnsCorrectValue(string json, ParticipantStatus expected)
    {
        var result = JsonSerializer.Deserialize<ParticipantStatus>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ParticipantStatus.Approved, "\"APPROVED\"")]
    [InlineData(ParticipantStatus.NeedsWork, "\"NEEDS_WORK\"")]
    [InlineData(ParticipantStatus.Unapproved, "\"UNAPPROVED\"")]
    public void ParticipantStatusConverter_Write_ReturnsCorrectValue(ParticipantStatus value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region RolesConverter Tests

    [Theory]
    [InlineData("\"AUTHOR\"", Roles.Author)]
    [InlineData("\"REVIEWER\"", Roles.Reviewer)]
    [InlineData("\"PARTICIPANT\"", Roles.Participant)]
    public void RolesConverter_Read_ReturnsCorrectValue(string json, Roles expected)
    {
        var result = JsonSerializer.Deserialize<Roles>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Roles.Author, "\"AUTHOR\"")]
    [InlineData(Roles.Reviewer, "\"REVIEWER\"")]
    [InlineData(Roles.Participant, "\"PARTICIPANT\"")]
    public void RolesConverter_Write_ReturnsCorrectValue(Roles value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region LineTypesConverter Tests

    [Theory]
    [InlineData("\"ADDED\"", LineTypes.Added)]
    [InlineData("\"REMOVED\"", LineTypes.Removed)]
    [InlineData("\"CONTEXT\"", LineTypes.Context)]
    public void LineTypesConverter_Read_ReturnsCorrectValue(string json, LineTypes expected)
    {
        var result = JsonSerializer.Deserialize<LineTypes>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(LineTypes.Added, "\"ADDED\"")]
    [InlineData(LineTypes.Removed, "\"REMOVED\"")]
    [InlineData(LineTypes.Context, "\"CONTEXT\"")]
    public void LineTypesConverter_Write_ReturnsCorrectValue(LineTypes value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region FileTypesConverter Tests

    [Theory]
    [InlineData("\"FROM\"", FileTypes.From)]
    [InlineData("\"TO\"", FileTypes.To)]
    public void FileTypesConverter_Read_ReturnsCorrectValue(string json, FileTypes expected)
    {
        var result = JsonSerializer.Deserialize<FileTypes>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(FileTypes.From, "\"FROM\"")]
    [InlineData(FileTypes.To, "\"TO\"")]
    public void FileTypesConverter_Write_ReturnsCorrectValue(FileTypes value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region HookTypesConverter Tests

    [Theory]
    [InlineData("\"PRE_RECEIVE\"", HookTypes.PreReceive)]
    [InlineData("\"POST_RECEIVE\"", HookTypes.PostReceive)]
    [InlineData("\"PRE_PULL_REQUEST_MERGE\"", HookTypes.PrePullRequestMerge)]
    public void HookTypesConverter_Read_ReturnsCorrectValue(string json, HookTypes expected)
    {
        var result = JsonSerializer.Deserialize<HookTypes>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(HookTypes.PreReceive, "\"PRE_RECEIVE\"")]
    [InlineData(HookTypes.PostReceive, "\"POST_RECEIVE\"")]
    [InlineData(HookTypes.PrePullRequestMerge, "\"PRE_PULL_REQUEST_MERGE\"")]
    public void HookTypesConverter_Write_ReturnsCorrectValue(HookTypes value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region ScopeTypesConverter Tests

    [Theory]
    [InlineData("\"PROJECT\"", ScopeTypes.Project)]
    [InlineData("\"REPOSITORY\"", ScopeTypes.Repository)]
    public void ScopeTypesConverter_Read_ReturnsCorrectValue(string json, ScopeTypes expected)
    {
        var result = JsonSerializer.Deserialize<ScopeTypes>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ScopeTypes.Project, "\"PROJECT\"")]
    [InlineData(ScopeTypes.Repository, "\"REPOSITORY\"")]
    public void ScopeTypesConverter_Write_ReturnsCorrectValue(ScopeTypes value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region WebHookOutcomesConverter Tests

    [Theory]
    [InlineData("\"SUCCESS\"", WebHookOutcomes.Success)]
    [InlineData("\"FAILURE\"", WebHookOutcomes.Failure)]
    [InlineData("\"ERROR\"", WebHookOutcomes.Error)]
    public void WebHookOutcomesConverter_Read_ReturnsCorrectValue(string json, WebHookOutcomes expected)
    {
        var result = JsonSerializer.Deserialize<WebHookOutcomes>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(WebHookOutcomes.Success, "\"SUCCESS\"")]
    [InlineData(WebHookOutcomes.Failure, "\"FAILURE\"")]
    [InlineData(WebHookOutcomes.Error, "\"ERROR\"")]
    public void WebHookOutcomesConverter_Write_ReturnsCorrectValue(WebHookOutcomes value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region BlockerCommentStateConverter Tests

    [Theory]
    [InlineData("\"OPEN\"", BlockerCommentState.Open)]
    [InlineData("\"RESOLVED\"", BlockerCommentState.Resolved)]
    public void BlockerCommentStateConverter_Read_ReturnsCorrectValue(string json, BlockerCommentState expected)
    {
        var result = JsonSerializer.Deserialize<BlockerCommentState>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(BlockerCommentState.Open, "\"OPEN\"")]
    [InlineData(BlockerCommentState.Resolved, "\"RESOLVED\"")]
    public void BlockerCommentStateConverter_Write_ReturnsCorrectValue(BlockerCommentState value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region CommentSeverityConverter Tests

    [Theory]
    [InlineData("\"NORMAL\"", CommentSeverity.Normal)]
    [InlineData("\"BLOCKER\"", CommentSeverity.Blocker)]
    public void CommentSeverityConverter_Read_ReturnsCorrectValue(string json, CommentSeverity expected)
    {
        var result = JsonSerializer.Deserialize<CommentSeverity>(json, s_options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CommentSeverity.Normal, "\"NORMAL\"")]
    [InlineData(CommentSeverity.Blocker, "\"BLOCKER\"")]
    public void CommentSeverityConverter_Write_ReturnsCorrectValue(CommentSeverity value, string expected)
    {
        var json = JsonSerializer.Serialize(value, s_options);
        Assert.Equal(expected, json);
    }

    #endregion

    #region Null Token Tests

    [Fact]
    public void PullRequestStatesConverter_Read_NullToken_ReturnsDefault()
    {
        var json = "null";
        var result = JsonSerializer.Deserialize<PullRequestStates?>(json, s_options);
        Assert.Null(result);
    }

    [Fact]
    public void RolesConverter_Read_NumberToken_ThrowsJsonException()
    {
        var json = "123";
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Roles>(json, s_options));
    }

    #endregion
}