using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace NexusMods.Telemetry.Metrics;

/// <summary>
/// Contains all available counters.
/// </summary>
[PublicAPI]
public static class Counters
{
    /// <summary>
    /// Creates an observable up-down counter for the number of active users.
    /// </summary>
    /// <remarks>
    /// This has already been configured to always return 1 and doesn't need to be
    /// updated any further by the caller.
    /// </remarks>
    public static ObservableUpDownCounter<int> CreateActiveUsersCounter(this Meter meter)
    {
        return meter.CreateObservableUpDownCounter(
            name: Constants.NameActiveUsersCounter,
            observeValue: ObserveActiveUsers
        );
    }

    private static int ObserveActiveUsers() => 1;

    /// <summary>
    /// Creates an observable up-down counter for the number of users per operating system.
    /// </summary>
    /// <remarks>
    /// This has already been configured and doesn't need to be updated any further by the
    /// caller.
    /// </remarks>
    public static ObservableUpDownCounter<int> CreateOperatingSystemCounter(this Meter meter)
    {
        return meter.CreateObservableUpDownCounter(
            name: Constants.NameUsersPerOS,
            observeValue: ObserveOperatingSystem
        );
    }

    private static string GetOperatingSystemInformation()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "Windows";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "Linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "macOS";
        return "Unknown";
    }

    private static readonly Measurement<int> OperatingSystemMeasurement = new(
        value: 1,
        tags: new KeyValuePair<string, object?>(Constants.TagOS, GetOperatingSystemInformation())
    );

    private static Measurement<int> ObserveOperatingSystem() => OperatingSystemMeasurement;
}
