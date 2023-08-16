using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NexusMods.Telemetry;

/// <summary>
/// Represents a collection of <see cref="TelemetryLibraryConfig"/>.
/// </summary>
[PublicAPI]
public record TelemetryLibraryConfigCollection
{
    /// <summary>
    /// All registered <see cref="TelemetryLibraryConfig"/>.
    /// </summary>
    public required List<TelemetryLibraryConfig> Configs { get; init; }
}

/// <summary>
/// Represents
/// </summary>
[PublicAPI]
public record TelemetryLibraryConfig
{
    /// <summary>
    /// Callback for configuring traces after DI initialization.
    /// </summary>
    public delegate void ConfigureTracesDelegate(IServiceProvider serviceProvider);

    /// <summary>
    /// Callback for configuring metrics after DI initialization.
    /// </summary>
    public delegate void ConfigureMetricsDelegate(IServiceProvider serviceProvider);

    /// <summary>
    /// Gets the <see cref="TelemetryLibraryInfo"/> of the current library.
    /// </summary>
    public required TelemetryLibraryInfo LibraryInfo { get; init; }

    /// <summary>
    /// Gets the optional callback to configure traces after DI initialization.
    /// </summary>
    public ConfigureTracesDelegate? ConfigureTraces { get; set; }

    /// <summary>
    /// Gets the optional callback to configure metrics after DI initialization.
    /// </summary>
    /// <remarks>
    /// This is helpful if you have observable instruments that create metrics from
    /// a service registered in DI.
    /// </remarks>
    public ConfigureMetricsDelegate? ConfigureMetrics { get; set; }
}
