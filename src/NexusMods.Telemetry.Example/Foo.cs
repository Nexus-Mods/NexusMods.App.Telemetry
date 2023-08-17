using System;
using JetBrains.Annotations;

namespace NexusMods.Telemetry.Example;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Foo
{
    public static string Bar()
    {
        using var trace = Telemetry.ActivitySource.StartActivity();

        try
        {
            SomethingThatThrows();
            return "baz";
        }
        catch (Exception e)
        {
            trace?.RecordException(e, "Exception while doing random stuff");
            return "unknown";
        }
    }

    private static void SomethingThatThrows()
    {
        if (Random.Shared.Next(0, 100) < 50) throw new NotSupportedException();
    }
}
