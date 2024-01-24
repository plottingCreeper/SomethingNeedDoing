using ECommons.DalamudServices;
using System.Numerics;

namespace SomethingNeedDoing.Misc.Commands;

internal class TargetStateCommands
{
    internal static TargetStateCommands Instance { get; } = new();
    public string GetTargetName() => Svc.Targets.Target?.Name.TextValue ?? "";
    public float GetTargetRawXPos() => Svc.Targets.Target?.Position.X ?? 0;
    public float GetTargetRawYPos() => Svc.Targets.Target?.Position.Y ?? 0;
    public float GetTargetRawZPos() => Svc.Targets.Target?.Position.Z ?? 0;

    public float GetDistanceToPoint(float x, float y, float z) => Vector3.Distance(Svc.ClientState.LocalPlayer!.Position, new Vector3(x, y, z));

    public float GetDistanceToTarget() => Vector3.Distance(Svc.ClientState.LocalPlayer!.Position, Svc.Targets.Target?.Position ?? Svc.ClientState.LocalPlayer!.Position);
}
