#nullable enable

using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class BitbucketHelpersTests
{
    #region Bool Tests

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void BoolToString_ReturnsCorrectValue(bool input, string expected)
    {
        var result = BitbucketHelpers.BoolToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    [InlineData(null, null)]
    public void BoolToString_Nullable_ReturnsCorrectValue(bool? input, string? expected)
    {
        var result = BitbucketHelpers.BoolToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region BranchOrderBy Tests

    [Theory]
    [InlineData(BranchOrderBy.Alphabetical, "ALPHABETICAL")]
    [InlineData(BranchOrderBy.Modification, "MODIFICATION")]
    public void BranchOrderByToString_ReturnsCorrectValue(BranchOrderBy input, string expected)
    {
        var result = BitbucketHelpers.BranchOrderByToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void BranchOrderByToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (BranchOrderBy)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.BranchOrderByToString(invalid));
    }

    #endregion

    #region PullRequestDirections Tests

    [Theory]
    [InlineData(PullRequestDirections.Incoming, "INCOMING")]
    [InlineData(PullRequestDirections.Outgoing, "OUTGOING")]
    public void PullRequestDirectionToString_ReturnsCorrectValue(PullRequestDirections input, string expected)
    {
        var result = BitbucketHelpers.PullRequestDirectionToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PullRequestDirectionToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (PullRequestDirections)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.PullRequestDirectionToString(invalid));
    }

    #endregion

    #region PullRequestStates Tests

    [Theory]
    [InlineData(PullRequestStates.Open, "OPEN")]
    [InlineData(PullRequestStates.Declined, "DECLINED")]
    [InlineData(PullRequestStates.Merged, "MERGED")]
    [InlineData(PullRequestStates.All, "ALL")]
    public void PullRequestStateToString_ReturnsCorrectValue(PullRequestStates input, string expected)
    {
        var result = BitbucketHelpers.PullRequestStateToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(PullRequestStates.Open, "OPEN")]
    [InlineData(null, null)]
    public void PullRequestStateToString_Nullable_ReturnsCorrectValue(PullRequestStates? input, string? expected)
    {
        var result = BitbucketHelpers.PullRequestStateToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region PullRequestOrders Tests

    [Theory]
    [InlineData(PullRequestOrders.Newest, "NEWEST")]
    [InlineData(PullRequestOrders.Oldest, "OLDEST")]
    public void PullRequestOrderToString_ReturnsCorrectValue(PullRequestOrders input, string expected)
    {
        var result = BitbucketHelpers.PullRequestOrderToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(PullRequestOrders.Newest, "NEWEST")]
    [InlineData(null, null)]
    public void PullRequestOrderToString_Nullable_ReturnsCorrectValue(PullRequestOrders? input, string? expected)
    {
        var result = BitbucketHelpers.PullRequestOrderToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PullRequestOrderToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (PullRequestOrders)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.PullRequestOrderToString(invalid));
    }

    #endregion

    #region PullRequestFromTypes Tests

    [Theory]
    [InlineData(PullRequestFromTypes.Comment, "COMMENT")]
    [InlineData(PullRequestFromTypes.Activity, "ACTIVITY")]
    [InlineData(null, null)]
    public void PullRequestFromTypeToString_Nullable_ReturnsCorrectValue(PullRequestFromTypes? input, string? expected)
    {
        var result = BitbucketHelpers.PullRequestFromTypeToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Permissions Tests

    [Theory]
    [InlineData(Permissions.Admin, "ADMIN")]
    [InlineData(Permissions.LicensedUser, "LICENSED_USER")]
    [InlineData(Permissions.ProjectAdmin, "PROJECT_ADMIN")]
    [InlineData(Permissions.ProjectCreate, "PROJECT_CREATE")]
    [InlineData(Permissions.ProjectRead, "PROJECT_READ")]
    [InlineData(Permissions.ProjectView, "PROJECT_VIEW")]
    [InlineData(Permissions.ProjectWrite, "PROJECT_WRITE")]
    [InlineData(Permissions.RepoAdmin, "REPO_ADMIN")]
    [InlineData(Permissions.RepoRead, "REPO_READ")]
    [InlineData(Permissions.RepoWrite, "REPO_WRITE")]
    [InlineData(Permissions.SysAdmin, "SYS_ADMIN")]
    public void PermissionToString_ReturnsCorrectValue(Permissions input, string expected)
    {
        var result = BitbucketHelpers.PermissionToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Permissions.Admin, "ADMIN")]
    [InlineData(null, null)]
    public void PermissionToString_Nullable_ReturnsCorrectValue(Permissions? input, string? expected)
    {
        var result = BitbucketHelpers.PermissionToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region MergeCommits Tests

    [Theory]
    [InlineData(MergeCommits.Exclude, "exclude")]
    [InlineData(MergeCommits.Include, "include")]
    [InlineData(MergeCommits.Only, "only")]
    public void MergeCommitsToString_ReturnsCorrectValue(MergeCommits input, string expected)
    {
        var result = BitbucketHelpers.MergeCommitsToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MergeCommitsToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (MergeCommits)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.MergeCommitsToString(invalid));
    }

    #endregion

    #region Roles Tests

    [Theory]
    [InlineData(Roles.Author, "AUTHOR")]
    [InlineData(Roles.Reviewer, "REVIEWER")]
    [InlineData(Roles.Participant, "PARTICIPANT")]
    public void RoleToString_ReturnsCorrectValue(Roles input, string expected)
    {
        var result = BitbucketHelpers.RoleToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Roles.Author, "AUTHOR")]
    [InlineData(null, null)]
    public void RoleToString_Nullable_ReturnsCorrectValue(Roles? input, string? expected)
    {
        var result = BitbucketHelpers.RoleToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region LineTypes Tests

    [Theory]
    [InlineData(LineTypes.Added, "ADDED")]
    [InlineData(LineTypes.Removed, "REMOVED")]
    [InlineData(LineTypes.Context, "CONTEXT")]
    public void LineTypeToString_ReturnsCorrectValue(LineTypes input, string expected)
    {
        var result = BitbucketHelpers.LineTypeToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(LineTypes.Added, "ADDED")]
    [InlineData(null, null)]
    public void LineTypeToString_Nullable_ReturnsCorrectValue(LineTypes? input, string? expected)
    {
        var result = BitbucketHelpers.LineTypeToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region FileTypes Tests

    [Theory]
    [InlineData(FileTypes.From, "FROM")]
    [InlineData(FileTypes.To, "TO")]
    public void FileTypeToString_ReturnsCorrectValue(FileTypes input, string expected)
    {
        var result = BitbucketHelpers.FileTypeToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(FileTypes.From, "FROM")]
    [InlineData(null, null)]
    public void FileTypeToString_Nullable_ReturnsCorrectValue(FileTypes? input, string? expected)
    {
        var result = BitbucketHelpers.FileTypeToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region ChangeScopes Tests

    [Theory]
    [InlineData(ChangeScopes.All, "ALL")]
    [InlineData(ChangeScopes.Unreviewed, "UNREVIEWED")]
    [InlineData(ChangeScopes.Range, "RANGE")]
    public void ChangeScopeToString_ReturnsCorrectValue(ChangeScopes input, string expected)
    {
        var result = BitbucketHelpers.ChangeScopeToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ChangeScopeToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (ChangeScopes)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.ChangeScopeToString(invalid));
    }

    #endregion

    #region ParticipantStatus Tests

    [Theory]
    [InlineData(ParticipantStatus.Approved, "APPROVED")]
    [InlineData(ParticipantStatus.NeedsWork, "NEEDS_WORK")]
    [InlineData(ParticipantStatus.Unapproved, "UNAPPROVED")]
    public void ParticipantStatusToString_ReturnsCorrectValue(ParticipantStatus input, string expected)
    {
        var result = BitbucketHelpers.ParticipantStatusToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region ArchiveFormats Tests

    [Theory]
    [InlineData(ArchiveFormats.Zip, "zip")]
    [InlineData(ArchiveFormats.Tar, "tar")]
    [InlineData(ArchiveFormats.TarGz, "tar.gz")]
    [InlineData(ArchiveFormats.Tgz, "tgz")]
    public void ArchiveFormatToString_ReturnsCorrectValue(ArchiveFormats input, string expected)
    {
        var result = BitbucketHelpers.ArchiveFormatToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ArchiveFormatToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (ArchiveFormats)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.ArchiveFormatToString(invalid));
    }

    #endregion

    #region WebHookOutcomes Tests

    [Theory]
    [InlineData(WebHookOutcomes.Success, "SUCCESS")]
    [InlineData(WebHookOutcomes.Failure, "FAILURE")]
    [InlineData(WebHookOutcomes.Error, "ERROR")]
    public void WebHookOutcomeToString_ReturnsCorrectValue(WebHookOutcomes input, string expected)
    {
        var result = BitbucketHelpers.WebHookOutcomeToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(WebHookOutcomes.Success, "SUCCESS")]
    [InlineData(null, null)]
    public void WebHookOutcomeToString_Nullable_ReturnsCorrectValue(WebHookOutcomes? input, string? expected)
    {
        var result = BitbucketHelpers.WebHookOutcomeToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region AnchorStates Tests

    [Theory]
    [InlineData(AnchorStates.Active, "ACTIVE")]
    [InlineData(AnchorStates.Orphaned, "ORPHANED")]
    [InlineData(AnchorStates.All, "ALL")]
    public void AnchorStateToString_ReturnsCorrectValue(AnchorStates input, string expected)
    {
        var result = BitbucketHelpers.AnchorStateToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AnchorStateToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (AnchorStates)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.AnchorStateToString(invalid));
    }

    #endregion

    #region DiffTypes Tests

    [Theory]
    [InlineData(DiffTypes.Effective, "EFFECTIVE")]
    [InlineData(DiffTypes.Range, "RANGE")]
    [InlineData(DiffTypes.Commit, "COMMIT")]
    public void DiffTypeToString_ReturnsCorrectValue(DiffTypes input, string expected)
    {
        var result = BitbucketHelpers.DiffTypeToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(DiffTypes.Effective, "EFFECTIVE")]
    [InlineData(null, null)]
    public void DiffTypeToString_Nullable_ReturnsCorrectValue(DiffTypes? input, string? expected)
    {
        var result = BitbucketHelpers.DiffTypeToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DiffTypeToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (DiffTypes)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.DiffTypeToString(invalid));
    }

    #endregion

    #region TagTypes Tests

    [Theory]
    [InlineData(TagTypes.LightWeight, "LIGHTWEIGHT")]
    [InlineData(TagTypes.Annotated, "ANNOTATED")]
    public void TagTypeToString_ReturnsCorrectValue(TagTypes input, string expected)
    {
        var result = BitbucketHelpers.TagTypeToString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TagTypeToString_InvalidValue_ThrowsArgumentException()
    {
        var invalid = (TagTypes)999;
        Assert.Throws<ArgumentException>(() => BitbucketHelpers.TagTypeToString(invalid));
    }

    #endregion

    #region RefRestrictionTypes Tests

    [Theory]
    [InlineData(RefRestrictionTypes.AllChanges, "read-only")]
    [InlineData(RefRestrictionTypes.RewritingHistory, "fast-forward-only")]
    [InlineData(RefRestrictionTypes.Deletion, "no-deletes")]
    [InlineData(RefRestrictionTypes.ChangesWithoutPullRequest, "pull-request-only")]
    public void RefRestrictionTypeToString_ReturnsCorrectValue(RefRestrictionTypes input, string expected)
    {
        var result = BitbucketHelpers.RefRestrictionTypeToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(RefRestrictionTypes.AllChanges, "read-only")]
    [InlineData(null, null)]
    public void RefRestrictionTypeToString_Nullable_ReturnsCorrectValue(RefRestrictionTypes? input, string? expected)
    {
        var result = BitbucketHelpers.RefRestrictionTypeToString(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region BlockerCommentState Tests

    [Theory]
    [InlineData(BlockerCommentState.Open, "OPEN")]
    [InlineData(BlockerCommentState.Resolved, "RESOLVED")]
    public void BlockerCommentStateToString_ReturnsCorrectValue(BlockerCommentState input, string expected)
    {
        var result = BitbucketHelpers.BlockerCommentStateToString(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(BlockerCommentState.Open, "OPEN")]
    [InlineData(null, null)]
    public void BlockerCommentStateToString_Nullable_ReturnsCorrectValue(BlockerCommentState? input, string? expected)
    {
        var result = BitbucketHelpers.BlockerCommentStateToString(input);
        Assert.Equal(expected, result);
    }

    #endregion
}