using System;
using System.Globalization;
using System.Threading;
using JetBrains.Annotations;

namespace NexusMods.Telemetry;

/// <summary>
/// Extension methods for <see cref="Exception"/>.
/// </summary>
[PublicAPI]
public static class ExceptionExtensions
{
    /// <summary>
    /// Returns a culture-independent string representation of the given <paramref name="exception"/> object,
    /// appropriate for diagnostics tracing.
    /// </summary>
    /// <param name="exception">Exception to convert to string.</param>
    /// <returns>Exception as string with no culture.</returns>
    public static string ToInvariantString(this Exception exception)
    {
        var originalUICulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            return exception.ToString();
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }
}
