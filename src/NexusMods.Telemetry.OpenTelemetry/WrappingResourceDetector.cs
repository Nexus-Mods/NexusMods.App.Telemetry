using OpenTelemetry.Resources;

namespace NexusMods.Telemetry.OpenTelemetry;

internal class WrappingResourceDetector : IResourceDetector
{
    private readonly Resource _resource;

    public WrappingResourceDetector(Resource resource)
    {
        _resource = resource;
    }

    public Resource Detect() => _resource;
}
