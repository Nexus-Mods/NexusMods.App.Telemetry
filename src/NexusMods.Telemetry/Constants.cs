using JetBrains.Annotations;
using NexusMods.Telemetry.Metrics;

namespace NexusMods.Telemetry;

/// <summary>
/// Includes constants for instrument and tag names.
/// </summary>
[PublicAPI]
public static class Constants
{
    /// <summary>
    /// The name of the <see cref="Counters.CreateActiveUsersCounter"/> counter.
    /// </summary>
    public const string NameActiveUsersCounter = "app_active_users";

    /// <summary>
    /// The name of the <see cref="Counters.CreateOperatingSystemCounter"/> counter.
    /// </summary>
    public const string NameUsersPerOS = "app_users_per_os";

    /// <summary>
    /// Tag that describes the current operating system.
    /// </summary>
    /// <remarks>
    /// The value can either be <c>Windows</c>, <c>Linux</c>, <c>macOS</c> or <c>Unknown</c>.
    /// </remarks>
    /// <seealso cref="Counters.CreateOperatingSystemCounter"/>
    public const string TagOS = "os";
}
