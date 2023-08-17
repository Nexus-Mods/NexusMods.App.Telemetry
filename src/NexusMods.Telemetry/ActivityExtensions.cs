using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NexusMods.Telemetry;

/// <summary>
/// Extension methods for <see cref="Activity"/>
/// </summary>
[PublicAPI]
public static class ActivityExtensions
{
    public static void RecordException(this Activity? activity, Exception exception, string description)
    {
        if (activity is null) return;
        activity.SetStatus(ActivityStatusCode.Error, description);

        var tagsCollection = new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.stacktrace", exception.ToInvariantString() },
        };

        if (!string.IsNullOrWhiteSpace(exception.Message))
        {
            tagsCollection.Add("exception.message", exception.Message);
        }

        activity.AddEvent(new ActivityEvent("exception", default, tagsCollection));
    }
}
