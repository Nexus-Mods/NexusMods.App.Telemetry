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
    /// The name of the <see cref="Counters.CreateLanguageCounter{TState}"/> counter.
    /// </summary>
    public const string NameUsersPerLanguage = "app_users_per_language";

    /// <summary>
    /// The name of the <see cref="Counters.CreateMembershipCounter{TState}"/> counter.
    /// </summary>
    public const string NameUsersPerMembership = "app_users_per_membership";

    /// <summary>
    /// Tag that describes the current operating system.
    /// </summary>
    /// <remarks>
    /// The value can either be <c>Windows</c>, <c>Linux</c>, <c>macOS</c> or <c>Unknown</c>.
    /// </remarks>
    /// <seealso cref="Counters.CreateOperatingSystemCounter"/>
    public const string TagOS = "os";

    /// <summary>
    /// Tag that describes the current selected language.
    /// </summary>
    public const string TagLanguage = "language";

    /// <summary>
    /// Tag that describes the current membership status of the user.
    /// </summary>
    public const string TagMembership = "membership";
}
