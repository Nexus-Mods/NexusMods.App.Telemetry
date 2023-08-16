using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NexusMods.Telemetry.OpenTelemetry;

/// <summary>
/// Static class for adding OpenTelemetry to a DI container.
/// </summary>
[PublicAPI]
public static class OpenTelemetryRegistration
{
    /// <summary>
    /// Adds OpenTelemetry to a DI container.
    /// </summary>
    public static IServiceCollection AddTelemetry(
        IServiceCollection serviceCollection,
        OpenTelemetrySettings settings)
    {
        // Since OpenTelemetry gets added to the DI container, it can't be disabled at runtime.
        // A custom written exporter could have a runtime check, however, this would be non-trivial
        // and reduce performance.
        // Instead, it's easier to have a single check at startup. If OpenTelemetry isn't added to the
        // DI container, then the SDK won't register listeners for Activities and Meters.
        // The downside is that the user has to restart the App for the changes to take effect.
        if (!settings.IsEnabled) return serviceCollection;

        var openTelemetryBuilder = serviceCollection.AddOpenTelemetry();

        // A "resource" is an entity for which telemetry is recorded.
        // https://opentelemetry.io/docs/concepts/glossary/#resource
        // In a cloud native environment, this would contain information
        // about the docker container, the cluster, the cloud provider,
        // and other details about the environment.
        // For a desktop app, we want to limit this information to
        // technical system information that don't impose heavy privacy
        // restrictions.
        openTelemetryBuilder.ConfigureResource(builder =>
        {
            // The SDK adds various details about the environment by default.
            // As explained earlier, we don't want this.
            builder.Clear();

            // Re-add details about the SDK (name, language and version):
            // https://github.com/open-telemetry/opentelemetry-dotnet/blob/031ed48714e16ba4a5b099b6e14647994a0b9c1b/src/OpenTelemetry/Resources/ResourceBuilderExtensions.cs#L30-L35
            builder.AddTelemetrySdk();

            var resource = new Resource(CreateAttributes(settings));

            // The SDK uses "detectors" to gather information about the current environment.
            // In our case, we just create and return a custom made resource.
            builder.AddDetector(new WrappingResourceDetector(resource));
        });

        if (settings.EnableTracing)
        {
            openTelemetryBuilder.WithTracing(builder =>
            {
                builder.AddOtlpExporter(exporterOptions => ConfigureOtlp(
                    exporterOptions,
                    settings,
                    configureTraces: true)
                );

                // Using the deferred provider we can register ActivitySources after the IServiceProvider has been created.
                if (builder is not IDeferredTracerProviderBuilder deferredBuilder) throw new NotSupportedException();
                deferredBuilder.Configure(ConfigureTracerProviderBuilder);
            });
        }

        if (settings.EnableMetrics)
        {
            openTelemetryBuilder.WithMetrics(builder =>
            {
                builder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                {
                    metricReaderOptions.PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
                    {
                        // This option allows us to change the interval at which observable instruments are
                        // getting observed. The SDK will observe the registered instruments every X milliseconds
                        // and send the metrics to the exporter.

                        // NOTE(erri120): This value must not change. See the telemetry backend
                        // documentation for more details.
                        ExportIntervalMilliseconds = (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                    };

                    ConfigureOtlp(exporterOptions, settings, configureTraces: false);
                });

                // Using the deferred provider we can register Meters after the IServiceProvider has been created.
                if (builder is not IDeferredMeterProviderBuilder deferredBuilder) throw new NotSupportedException();
                deferredBuilder.Configure(ConfigureMeterProviderBuilder);
            });
        }

        return serviceCollection;
    }

    private static IEnumerable<KeyValuePair<string, object>> CreateAttributes(OpenTelemetrySettings settings)
    {
        // "conventional keys" taken from
        // https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry/Internal/ResourceSemanticConventions.cs

        yield return new KeyValuePair<string, object>("service.name", settings.ApplicationName);
        yield return new KeyValuePair<string, object>("service.version", settings.ApplicationVersion.ToString());

        // The "service.instance.id" is very important and frequent changes to this can lead to an explosion
        // of new unique time series.
        yield return new KeyValuePair<string, object>("service.instance.id", settings.ApplicationVersion.ToString());
    }

    private static void ConfigureOtlp(
        OtlpExporterOptions exporterOptions,
        OpenTelemetrySettings settings,
        bool configureTraces)
    {
        exporterOptions.TimeoutMilliseconds = (int)TimeSpan.FromSeconds(2).TotalMilliseconds;

        exporterOptions.Protocol = settings.ExporterProtocol;

        exporterOptions.Endpoint = configureTraces
            ? settings.ExporterTracesEndpoint
            : settings.ExporterMetricsEndpoint;
    }

    private static void ConfigureTracerProviderBuilder(
        IServiceProvider serviceProvider,
        TracerProviderBuilder tracerProviderBuilder)
    {
        var configCollection = serviceProvider.GetRequiredService<TelemetryLibraryConfigCollection>();
        var configs = configCollection.Configs;

        // The SDK requires that we know all names of all ActivitySources we want to use, up front.
        // Using the deferred builder, we can get those names from the DI system and add it after
        // construction to the provider.
        var names = configs.Select(x => x.LibraryInfo.AssemblyName).ToArray();
        tracerProviderBuilder.AddSource(names);

        foreach (var config in configs)
        {
            config.ConfigureTraces?.Invoke(serviceProvider);
        }
    }

    private static void ConfigureMeterProviderBuilder(
        IServiceProvider serviceProvider,
        MeterProviderBuilder meterProviderBuilder)
    {
        var configCollection = serviceProvider.GetRequiredService<TelemetryLibraryConfigCollection>();
        var configs = configCollection.Configs;

        // The SDK requires that we know all names of all Meters we want to use, up front.
        // Using the deferred builder, we can get those names from the DI system and add it after
        // construction to the provider.
        var names = configs.Select(x => x.LibraryInfo.AssemblyName).ToArray();
        meterProviderBuilder.AddMeter(names);

        foreach (var config in configs)
        {
            // This callback is essential for observable instruments that require services from DI.
            config.ConfigureMetrics?.Invoke(serviceProvider);
        }
    }
}
