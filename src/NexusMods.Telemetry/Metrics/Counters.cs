using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;

namespace NexusMods.Telemetry.Metrics;

/// <summary>
/// Contains all available counters.
/// </summary>
[PublicAPI]
public static class Counters
{
    #region Active Users

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

    #endregion

    #region Operating System

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
        // NOTE(erri120): Because Windows is just weird, there is no easy way
        // to extract the release number, eg: "Windows 10" or "Windows 11".
        // The best you can do is get the kernel version, which is super jank:
        // https://learn.microsoft.com/en-us/dotnet/api/system.environment.osversion
        // https://learn.microsoft.com/en-us/windows/win32/sysinfo/operating-system-version
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

    #endregion

    #region Language

    /// <summary>
    /// Gets the current selected language by the user.
    /// </summary>
    public delegate CultureInfo GetCurrentLanguage<in TState>(TState state);

    /// <summary>
    /// Implementation of <see cref="GetCurrentLanguage{TState}"/> that uses <see cref="Thread.CurrentUICulture"/>.
    /// </summary>
    public static readonly GetCurrentLanguage<NoState> GetCurrentUILanguage = _ => Thread.CurrentThread.CurrentUICulture;

    /// <summary>
    /// Creates an observable up-down counter for the number of users per language.
    /// </summary>
    public static ObservableUpDownCounter<int> CreateLanguageCounter<TState>(
        this Meter meter,
        GetCurrentLanguage<TState> getCurrentLanguage,
        TState state)
    {
        return meter.CreateObservableUpDownCounter(
            name: Constants.NameUsersPerLanguage,
            observeValue: () => ObserveLanguage(getCurrentLanguage, state)
        );
    }

    private static Measurement<int> ObserveLanguage<TState>(GetCurrentLanguage<TState> getCurrentLanguage, TState state)
    {
        var currentLanguage = getCurrentLanguage(state);

        return new Measurement<int>(
            value: 1,
            tags: new KeyValuePair<string, object?>(Constants.TagLanguage, currentLanguage.Name)
        );
    }

    #endregion

    #region Membership

    /// <summary>
    /// Gets the current membership status of the user.
    /// </summary>
    public delegate string GetCurrentMembership<in TState>(TState state);

    /// <summary>
    /// Creates an observable up-down counter for the number of users per membership.
    /// </summary>
    public static ObservableUpDownCounter<int> CreateMembershipCounter<TState>(
        this Meter meter,
        GetCurrentMembership<TState> getCurrentMembership,
        TState state)
    {
        return meter.CreateObservableUpDownCounter(
            name: Constants.NameUsersPerMembership,
            observeValue: () => ObserveMembership(getCurrentMembership, state)
        );
    }

    private static Measurement<int> ObserveMembership<TState>(GetCurrentMembership<TState> getCurrentMembership, TState state)
    {
        var membershipStatus = getCurrentMembership(state);

        return new Measurement<int>(
            value: 1,
            tags: new KeyValuePair<string, object?>(Constants.TagMembership, membershipStatus)
        );
    }

    #endregion
}
