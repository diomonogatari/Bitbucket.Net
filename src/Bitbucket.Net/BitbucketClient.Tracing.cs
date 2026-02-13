using Flurl.Http;
using System.Diagnostics;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    internal static readonly ActivitySource ActivitySource = new("Bitbucket.Net");

    private static readonly HttpRequestOptionsKey<Activity> s_activityKey = new("Bitbucket.Net.Activity");

    private static void OnBeforeCall(FlurlCall call)
    {
        if (!ActivitySource.HasListeners())
        {
            return;
        }

        var httpMethod = call.Request.Verb.Method;
        var activity = ActivitySource.StartActivity(httpMethod, ActivityKind.Client);

        if (activity is null)
        {
            return;
        }

        var url = call.Request.Url;
        activity.SetTag("http.request.method", httpMethod);
        activity.SetTag("url.full", url.ToString());
        activity.SetTag("server.address", url.Host);

        if (url.Port is not null)
        {
            activity.SetTag("server.port", url.Port.Value);
        }

        SetBitbucketTags(activity, url.Path);

        // Store the activity on the request message so we can retrieve it in AfterCall/OnError.
        // Activity.Current is not reliable here because Flurl's RaiseEventAsync is async,
        // and AsyncLocal changes inside async methods don't propagate to the caller.
        call.HttpRequestMessage.Options.Set(s_activityKey, activity);
    }

    private static void OnAfterCall(FlurlCall call)
    {
        if (!call.HttpRequestMessage.Options.TryGetValue(s_activityKey, out var activity))
        {
            return;
        }

        if (call.Response is not null)
        {
            var statusCode = call.Response.StatusCode;
            activity.SetTag("http.response.status_code", statusCode);

            if (statusCode >= 400)
            {
                activity.SetStatus(ActivityStatusCode.Error);
                activity.SetTag("error.type", statusCode.ToString());
            }
        }

        activity.Stop();
    }

    private static void OnErrorCall(FlurlCall call)
    {
        if (!call.HttpRequestMessage.Options.TryGetValue(s_activityKey, out var activity))
        {
            return;
        }

        if (call.Exception is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error, call.Exception.Message);
            activity.SetTag("error.type", call.Exception.GetType().FullName);
            activity.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
            {
                { "exception.type", call.Exception.GetType().FullName },
                { "exception.message", call.Exception.Message },
            }));
        }

        activity.Stop();
    }

    private static void SetBitbucketTags(Activity activity, string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < segments.Length - 1; i++)
        {
            if (string.Equals(segments[i], "projects", StringComparison.OrdinalIgnoreCase))
            {
                activity.SetTag("bitbucket.project_key", segments[i + 1]);
            }
            else if (string.Equals(segments[i], "repos", StringComparison.OrdinalIgnoreCase))
            {
                activity.SetTag("bitbucket.repository_slug", segments[i + 1]);
            }
        }
    }
}