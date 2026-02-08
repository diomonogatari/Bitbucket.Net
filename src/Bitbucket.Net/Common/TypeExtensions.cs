using System.Reflection;

namespace Bitbucket.Net.Common;

/// <summary>
/// Provides reflection-based helpers for working with <see cref="Type"/> instances.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="Type"/> is a <see cref="Nullable{T}"/>.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when the type is a nullable value type; otherwise <see langword="false"/>.</returns>
    public static bool IsNullableType(Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
}