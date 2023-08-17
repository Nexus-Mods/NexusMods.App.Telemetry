using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NexusMods.Telemetry.Metrics;

namespace NexusMods.Telemetry.Example;

internal static class Telemetry
{
    private const string AssemblyName = "NexusMods.Telemetry.Example";
    private static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0);
    private static readonly string AssemblyVersionString = AssemblyVersion.ToString(fieldCount: 3);

    internal static readonly TelemetryLibraryInfo LibraryInfo = new()
    {
        AssemblyName = AssemblyName,
        AssemblyVersion = AssemblyVersion
    };

    private static readonly Meter Meter = new(name: AssemblyName, version: AssemblyVersionString);

    internal static readonly ActivitySource ActivitySource = new(name: AssemblyName, version: AssemblyVersionString);

    // Create an observable counter that doesn't require DI
    [UsedImplicitly]
    private static readonly ObservableUpDownCounter<int> ActiveUsersCounter = Meter.CreateActiveUsersCounter();

    [UsedImplicitly]
    private static DIAwareMetricManager? _metricManager;

    internal static void SetupTelemetry(IServiceProvider serviceProvider)
    {
        // If you need DI, you can use the callback from ConfigureLibrary
        // See Services.cs for details
        _metricManager = new DIAwareMetricManager(serviceProvider, Meter);
    }

    private class DIAwareMetricManager
    {
        [UsedImplicitly]
        private readonly ObservableUpDownCounter<int> _membershipCounter;

        public DIAwareMetricManager(IServiceProvider serviceProvider, Meter meter)
        {
            // We can now use services from DI to observe values
            var membershipService = serviceProvider.GetRequiredService<MembershipService>();
            _membershipCounter = meter.CreateMembershipCounter(membershipService, service => service.GetMembershipStatus());
        }
    }
}
