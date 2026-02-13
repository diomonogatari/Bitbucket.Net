using System.Runtime.InteropServices;

namespace Bitbucket.Net.Common;

/// <summary>
/// Lightweight pagination metadata extracted from a paged API response.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly record struct PagedMetadata(
    bool IsLastPage,
    int? NextPageStart,
    int? Start,
    int? Limit,
    int Size);