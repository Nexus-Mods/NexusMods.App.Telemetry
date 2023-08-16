using System;
using JetBrains.Annotations;

namespace NexusMods.Telemetry;

/// <summary>
/// Represents information about the current library in our telemetry system.
/// </summary>
[PublicAPI]
public record TelemetryLibraryInfo
{
    /// <summary>
    /// Gets the name of the library/assembly.
    /// </summary>
    /// <example><c>NexusMods.Paths</c></example>
    public required string AssemblyName { get; init; }

    /// <summary>
    /// Gets the version of the library/assembly.
    /// </summary>
    /// <example><c>1.0.0.0</c></example>
    public required Version AssemblyVersion { get; init; }
}
