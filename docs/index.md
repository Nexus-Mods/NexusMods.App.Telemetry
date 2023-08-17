## For Developers

The project that creates the DI container has to include `NexusMods.Telemetry.OpenTelemetry` and use the `AddTelemetry` extension method to set up OpenTelemetry.

Libraries that want to expose metrics only need to include `NexusMods.Telemetry`. Create an internal class called `Telemetry`:

```csharp
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
}
```

This is boilerplate code that you can just copy. See [`Telemetry.cs`](../src/NexusMods.Telemetry.Example/Telemetry.cs) for an example. Make sure to replace `AssemblyName` with the current name of the library.

Now you only need to register your library in DI:

```csharp
public static class Services
{
    public static IServiceCollection AddExample(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .ConfigureTelemetry(
                Telemetry.LibraryInfo
            );
    }
}
```

You can use the `ActivitySource` to create traces:

```csharp
using var trace = Telemetry.ActivitySource.StartActivity();
```

The `Meter` can be used to create counters and other instruments:

```csharp
private static readonly Meter Meter = new(name: AssemblyName, version: AssemblyVersionString);

[UsedImplicitly]
private static readonly ObservableUpDownCounter<int> ActiveUsersCounter = Meter.CreateActiveUsersCounter();
```

If you need to use a service from DI for your metrics, you can use the callback when registering the telemetry:

```csharp
return serviceCollection
    .ConfigureTelemetry(
        Telemetry.LibraryInfo,
        // this callback allows us to use services from DI in our metrics
        configureMetrics: Telemetry.SetupTelemetry
    );
```

```csharp
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
```

This solution isn't elegant, but it's the only solution that actually works with OpenTelemetry and DI.

## For Users

TODO
