using System;

namespace NexusMods.Telemetry.Example;

public class MembershipService
{
    public string GetMembershipStatus() => Random.Shared.Next(0, 100) < 50 ? "None" : "Premium";
}
