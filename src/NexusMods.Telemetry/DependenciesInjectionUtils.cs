using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NexusMods.Telemetry;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
[PublicAPI]
public static class DependenciesInjectionUtils
{
    private static readonly TelemetryLibraryConfigCollection Collection = new()
    {
        Configs = new()
    };

    /// <summary>
    /// Configures telemetry for the current library.
    /// </summary>
    public static IServiceCollection ConfigureTelemetry(
        this IServiceCollection serviceCollection,
        TelemetryLibraryInfo libraryInfo,
        TelemetryLibraryConfig.ConfigureTracesDelegate? configureTraces = null,
        TelemetryLibraryConfig.ConfigureMetricsDelegate? configureMetrics = null)
    {
        return ConfigureTelemetry(serviceCollection, new TelemetryLibraryConfig
        {
            LibraryInfo = libraryInfo,
            ConfigureTraces = configureTraces,
            ConfigureMetrics = configureMetrics
        });
    }

    /// <summary>
    /// Configures telemetry for the current library.
    /// </summary>
    public static IServiceCollection ConfigureTelemetry(
        this IServiceCollection serviceCollection,
        TelemetryLibraryConfig config)
    {
        // NOTE(erri120): this isn't the most elegant solution, but it works for now.
        Collection.Configs.Add(config);

        serviceCollection.TryAddSingleton(Collection);
        return serviceCollection;
    }
}
