using System.Collections;

namespace Bitbucket.Net.Common;

/// <summary>
/// A helper for building multipart/form-data payloads where parts are added conditionally.
/// </summary>
public class DynamicMultipartFormDataContent : IEnumerable<HttpContent>
{
    private readonly MultipartFormDataContent _multipartFormDataContent = new MultipartFormDataContent();

    /// <summary>
    /// Adds a required multipart section.
    /// </summary>
    /// <param name="value">The HTTP content to add.</param>
    /// <param name="key">The form field name.</param>
    public void Add(HttpContent value, string key)
    {
        _multipartFormDataContent.Add(value, key);
    }

    /// <summary>
    /// Adds a multipart section when a value is provided and not equal to the default for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the guard value.</typeparam>
    /// <param name="t">The guard value to test.</param>
    /// <param name="value">The HTTP content to add when <paramref name="t"/> is present.</param>
    /// <param name="key">The form field name.</param>
    public void Add<T>(T t, HttpContent? value, string key)
    {
        if (!EqualityComparer<T>.Default.Equals(t, default) && value is not null)
        {
            _multipartFormDataContent.Add(value, key);
        }
    }

    /// <summary>
    /// Finalizes the builder and returns the underlying <see cref="MultipartFormDataContent"/> instance.
    /// </summary>
    /// <returns>The built multipart form data content.</returns>
    public MultipartFormDataContent ToMultipartFormDataContent() => _multipartFormDataContent;

    /// <inheritdoc />
    public IEnumerator<HttpContent> GetEnumerator()
    {
        return _multipartFormDataContent.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}