#nullable enable

using Bitbucket.Net.Serialization;
using System.Reflection;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

/// <summary>
/// Verifies that all model types are registered in <see cref="BitbucketJsonContext"/>.
/// Without this test, a missing [JsonSerializable] attribute would cause a runtime
/// <see cref="NotSupportedException"/> instead of being caught at build time.
/// </summary>
public class SourceGenCoverageTests
{
    private static readonly Assembly s_libraryAssembly = typeof(BitbucketClient).Assembly;

    /// <summary>
    /// Enumerates every public, non-abstract, non-enum class in the Bitbucket.Net.Models
    /// and Bitbucket.Net.Common.Models namespaces and asserts that the source-generated
    /// context can resolve type info for each.
    /// </summary>
    [Fact]
    public void AllModelTypesRegisteredInSourceGenContext()
    {
        var modelTypes = GetModelTypes();

        Assert.NotEmpty(modelTypes);

        var missing = new List<string>();

        foreach (var type in modelTypes)
        {
            try
            {
                var typeInfo = BitbucketJsonContext.Default.GetTypeInfo(type);
                if (typeInfo is null)
                {
                    missing.Add(type.FullName!);
                }
            }
            catch
            {
                missing.Add(type.FullName!);
            }
        }

        Assert.True(
            missing.Count == 0,
            $"The following model types are not registered in BitbucketJsonContext:\n{string.Join("\n", missing)}");
    }

    /// <summary>
    /// Verifies that PagedResults&lt;T&gt; generic instantiations are registered
    /// for the primary model types used in paginated API responses.
    /// </summary>
    [Theory]
    [MemberData(nameof(PagedResultTypes))]
    public void PagedResultsTypeIsRegistered(Type itemType)
    {
        var pagedType = typeof(Bitbucket.Net.Common.Models.PagedResults<>).MakeGenericType(itemType);

        var typeInfo = BitbucketJsonContext.Default.GetTypeInfo(pagedType);

        Assert.NotNull(typeInfo);
    }

    public static TheoryData<Type> PagedResultTypes()
    {
        var data = new TheoryData<Type>();

        foreach (var t in GetKnownPagedItemTypes())
        {
            data.Add(t);
        }

        return data;
    }

    private static Type[] GetKnownPagedItemTypes()
    {
        return
        [
            typeof(Bitbucket.Net.Models.PersonalAccessTokens.AccessToken),
            typeof(Bitbucket.Net.Models.Audit.AuditEvent),
            typeof(Bitbucket.Net.Models.Core.Tasks.BitbucketTask),
            typeof(Bitbucket.Net.Models.Core.Projects.BlockerComment),
            typeof(Bitbucket.Net.Models.Core.Projects.Branch),
            typeof(Bitbucket.Net.Models.Core.Projects.BranchBase),
            typeof(Bitbucket.Net.Models.Builds.BuildStatus),
            typeof(Bitbucket.Net.Models.Core.Projects.Change),
            typeof(Bitbucket.Net.Models.Jira.ChangeSet),
            typeof(Bitbucket.Net.Models.Core.Projects.Comment),
            typeof(Bitbucket.Net.Models.Core.Projects.CommentRef),
            typeof(Bitbucket.Net.Models.Core.Projects.Commit),
            typeof(Bitbucket.Net.Models.Core.Projects.ContentItem),
            typeof(Bitbucket.Net.Models.Core.Admin.DeletableGroupOrUser),
            typeof(Bitbucket.Net.Models.Core.Admin.GroupPermission),
            typeof(Bitbucket.Net.Models.Core.Projects.Hook),
            typeof(Bitbucket.Net.Models.Core.Users.Identity),
            typeof(Bitbucket.Net.Models.RefRestrictions.Key),
            typeof(Bitbucket.Net.Models.Core.Projects.LicensedUser),
            typeof(Bitbucket.Net.Models.Core.Projects.Participant),
            typeof(Bitbucket.Net.Models.Core.Projects.Project),
            typeof(Bitbucket.Net.Models.Ssh.ProjectKey),
            typeof(Bitbucket.Net.Models.Core.Projects.PullRequest),
            typeof(Bitbucket.Net.Models.Core.Projects.PullRequestActivity),
            typeof(Bitbucket.Net.Models.Core.Projects.PullRequestSuggestion),
            typeof(Bitbucket.Net.Models.RefRestrictions.RefRestriction),
            typeof(Bitbucket.Net.Models.Core.Projects.Repository),
            typeof(Bitbucket.Net.Models.Core.Projects.RepositoryFork),
            typeof(Bitbucket.Net.Models.Ssh.RepositoryKey),
            typeof(string),
            typeof(Bitbucket.Net.Models.Core.Projects.Tag),
            typeof(Bitbucket.Net.Models.Core.Users.User),
            typeof(Bitbucket.Net.Models.Core.Admin.UserInfo),
            typeof(Bitbucket.Net.Models.Core.Admin.UserPermission),
            typeof(Bitbucket.Net.Models.Core.Projects.WebHook),
        ];
    }

    private static List<Type> GetModelTypes()
    {
        return s_libraryAssembly.GetExportedTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                !t.IsGenericTypeDefinition &&
                t.Namespace is not null &&
                (t.Namespace.StartsWith("Bitbucket.Net.Models", StringComparison.Ordinal) ||
                 t.Namespace.StartsWith("Bitbucket.Net.Common.Models", StringComparison.Ordinal)))
            .ToList();
    }
}