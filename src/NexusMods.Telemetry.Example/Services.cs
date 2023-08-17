using Microsoft.Extensions.DependencyInjection;

namespace NexusMods.Telemetry.Example;

public static class Services
{
    public static IServiceCollection AddExample(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<Foo>()
            .AddSingleton<MembershipService>()
            .ConfigureTelemetry(
                Telemetry.LibraryInfo,
                // this callback allows us to use services from DI in our metrics
                configureMetrics: Telemetry.SetupTelemetry
            );
    }
}
