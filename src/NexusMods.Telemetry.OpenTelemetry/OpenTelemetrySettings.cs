using System;
using JetBrains.Annotations;
using OpenTelemetry.Exporter;

namespace NexusMods.Telemetry.OpenTelemetry;

/// <summary>
/// Setting used by <see cref="OpenTelemetryRegistration"/>.
/// </summary>
[PublicAPI]
public sealed record OpenTelemetrySettings
{
    /// <summary>
    /// Whether or not telemetry is enabled.
    /// </summary>
    public required bool IsEnabled { get; init; }

    /// <summary>
    /// Whether or not tracing is enabled.
    /// </summary>
    public required bool EnableTracing { get; init; }

    /// <summary>
    /// Whether or not metrics are enabled.
    /// </summary>
    public required bool EnableMetrics { get; init; }

    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    public required string ApplicationName { get; init; }

    /// <summary>
    /// Gets the current version of the application.
    /// </summary>
    public required Version ApplicationVersion { get; init; }

    /// <summary>
    /// Gets the protocol to use for the OTLP Exporter.
    /// </summary>
    public required OtlpExportProtocol ExporterProtocol { get; init; }

    /// <summary>
    /// Gets the URI to use for the OTLP Exporter when exporting traces.
    /// </summary>
    public required Uri ExporterTracesEndpoint { get; init; }

    /// <summary>
    /// Gets the URI to use for the OTLP Exporter when exporting metrics.
    /// </summary>
    public required Uri ExporterMetricsEndpoint { get; init; }
}
