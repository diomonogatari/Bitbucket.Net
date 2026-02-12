using Bitbucket.Net.Models.Core.Projects;
using NSubstitute;
using System.Reflection;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class InterfaceTests
{
    private static readonly Type s_interfaceType = typeof(IBitbucketClient);
    private static readonly Type s_concreteType = typeof(BitbucketClient);

    [Fact]
    public void IBitbucketClient_HasAllPublicMethodsFromBitbucketClient()
    {
        var concretePublicMethods = s_concreteType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName) // exclude property accessors
            .Where(m => m.Name is not "Dispose")
            .Select(m => GetMethodSignature(m))
            .ToHashSet(StringComparer.Ordinal);

        var interfaceMethods = s_interfaceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.IsSpecialName)
            .Where(m => m.DeclaringType != typeof(IDisposable))
            .Select(m => GetMethodSignature(m))
            .ToHashSet(StringComparer.Ordinal);

        var missingFromInterface = concretePublicMethods.Except(interfaceMethods).ToList();
        var extraOnInterface = interfaceMethods.Except(concretePublicMethods).ToList();

        Assert.True(
            missingFromInterface.Count == 0,
            $"Methods on BitbucketClient missing from IBitbucketClient:\n{string.Join('\n', missingFromInterface)}");

        Assert.True(
            extraOnInterface.Count == 0,
            $"Methods on IBitbucketClient not found on BitbucketClient:\n{string.Join('\n', extraOnInterface)}");
    }

    [Fact]
    public void IBitbucketClient_PreservesObsoleteAttributes()
    {
        var obsoleteMethods = s_interfaceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<ObsoleteAttribute>() is not null)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        Assert.Equal(
            ["GetPullRequestTaskCountAsync", "GetPullRequestTasksAsync", "GetPullRequestTasksStreamAsync"],
            obsoleteMethods);
    }

    [Fact]
    public async Task IBitbucketClient_IsMockableWithNSubstitute()
    {
        var mock = Substitute.For<IBitbucketClient>();

        IReadOnlyList<Project> expected = [new Project { Key = "TEST", Name = "Test Project" }];
        mock.GetProjectsAsync().Returns(Task.FromResult(expected));

        var result = await mock.GetProjectsAsync();

        Assert.Single(result);
        Assert.Equal("TEST", result[0].Key);
    }

    [Fact]
    public void BitbucketClient_IsAssignableToIBitbucketClient()
    {
        Assert.True(s_interfaceType.IsAssignableFrom(s_concreteType));
    }

    private static string GetMethodSignature(MethodInfo m)
    {
        var parameters = string.Join(", ", m.GetParameters().Select(p => $"{FormatType(p.ParameterType)} {p.Name}"));
        return $"{FormatType(m.ReturnType)} {m.Name}({parameters})";
    }

    private static string FormatType(Type t)
    {
        if (!t.IsGenericType)
        {
            return t.FullName ?? t.Name;
        }

        var genericDef = t.GetGenericTypeDefinition().FullName!;
        var baseName = genericDef[..genericDef.IndexOf('`')];
        var args = string.Join(", ", t.GetGenericArguments().Select(FormatType));
        return $"{baseName}<{args}>";
    }
}