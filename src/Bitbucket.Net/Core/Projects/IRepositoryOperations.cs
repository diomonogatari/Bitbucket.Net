using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net;

/// <summary>
/// Repository management operations.
/// </summary>
public interface IRepositoryOperations
{
    Task<IReadOnlyList<Repository>> GetProjectRepositoriesAsync(string projectKey, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Repository> GetProjectRepositoriesStreamAsync(string projectKey, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Repository> CreateProjectRepositoryAsync(string projectKey, CreateRepositoryRequest request, CancellationToken cancellationToken = default);
    Task<Repository> GetProjectRepositoryAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<RepositoryFork> CreateProjectRepositoryForkAsync(string projectKey, string repositorySlug, ForkRepositoryRequest? request = null, CancellationToken cancellationToken = default);
    Task<bool> ScheduleProjectRepositoryForDeletionAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<Repository> UpdateProjectRepositoryAsync(string projectKey, string repositorySlug, string? targetName = null, bool? isForkable = null, string? targetProjectKey = null, bool? isPublic = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryFork>> GetProjectRepositoryForksAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Repository> RecreateProjectRepositoryAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryFork>> GetRelatedProjectRepositoriesAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<byte[]> GetProjectRepositoryArchiveAsync(string projectKey, string repositorySlug, string at, string fileName, ArchiveFormats archiveFormat, string path, string prefix, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPermission>> GetProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetProjectRepositoryGroupPermissionsNoneAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermission>> GetProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, string name, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetProjectRepositoryUserPermissionsNoneAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Stream> RetrieveRawContentAsync(string projectKey, string repositorySlug, string path, string? at = null, bool markup = false, bool hardWrap = true, bool htmlEscape = true, CancellationToken cancellationToken = default);
    Task<PullRequestSettings> GetProjectRepositoryPullRequestSettingsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<PullRequestSettings> UpdateProjectRepositoryPullRequestSettingsAsync(string projectKey, string repositorySlug, PullRequestSettings pullRequestSettings, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Hook>> GetProjectRepositoryHooksSettingsAsync(string projectKey, string repositorySlug, HookTypes? hookType = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Hook> GetProjectRepositoryHookSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryHookSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<Hook> EnableProjectRepositoryHookAsync(string projectKey, string repositorySlug, string hookKey, object? hookSettings = null, CancellationToken cancellationToken = default);
    Task<Hook> DisableProjectRepositoryHookAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object?>> GetProjectRepositoryHookAllSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object?>> UpdateProjectRepositoryHookAllSettingsAsync(string projectKey, string repositorySlug, string hookKey, Dictionary<string, object?> allSettings, CancellationToken cancellationToken = default);
    Task<PullRequestSettings> GetProjectPullRequestsMergeStrategiesAsync(string projectKey, string scmId, CancellationToken cancellationToken = default);
    Task<MergeStrategies> UpdateProjectPullRequestsMergeStrategiesAsync(string projectKey, string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> GetProjectRepositoryTagsAsync(string projectKey, string repositorySlug, string filterText, BranchOrderBy orderBy, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Tag> GetProjectRepositoryTagsStreamAsync(string projectKey, string repositorySlug, string filterText, BranchOrderBy orderBy, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Tag> CreateProjectRepositoryTagAsync(string projectKey, string repositorySlug, string name, string startPoint, string message, CancellationToken cancellationToken = default);
    Task<Tag> GetProjectRepositoryTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WebHook>> GetProjectRepositoryWebHooksAsync(string projectKey, string repositorySlug, string? @event = null, bool statistics = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<WebHook> CreateProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, CreateWebHookRequest request, CancellationToken cancellationToken = default);
    Task<WebHookTestRequestResponse> TestProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string url, CancellationToken cancellationToken = default);
    Task<WebHook> GetProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string webHookId, bool statistics = false, CancellationToken cancellationToken = default);
    Task<WebHook> UpdateProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string webHookId, UpdateWebHookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string webHookId, CancellationToken cancellationToken = default);
    Task<string> GetProjectRepositoryWebHookLatestAsync(string projectKey, string repositorySlug, string webHookId, string? @event = null, WebHookOutcomes? outcome = null, CancellationToken cancellationToken = default);
    Task<WebHookStatistics> GetProjectRepositoryWebHookStatisticsAsync(string projectKey, string repositorySlug, string webHookId, string? @event = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, WebHookStatisticsCounts>> GetProjectRepositoryWebHookStatisticsSummaryAsync(string projectKey, string repositorySlug, string webHookId, CancellationToken cancellationToken = default);
}