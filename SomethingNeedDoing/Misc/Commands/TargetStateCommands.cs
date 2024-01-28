using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace SomethingNeedDoing.Misc.Commands;

internal class TargetStateCommands
{
    internal static TargetStateCommands Instance { get; } = new();

    public List<string> ListAllFunctions()
    {
        MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        var list = new List<string>();
        foreach (MethodInfo method in methods.Where(x => x.Name != nameof(ListAllFunctions) && x.DeclaringType != typeof(object)))
        {
            var parameterList = method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}{(p.IsOptional ? " = " + (p.DefaultValue ?? "null") : "")}");
            list.Add($"{method.ReturnType.Name} {method.Name}({string.Join(", ", parameterList)})");
        }
        return list;
    }

    public string GetTargetName() => Svc.Targets.Target?.Name.TextValue ?? "";
    public float GetTargetRawXPos() => Svc.Targets.Target?.Position.X ?? 0;
    public float GetTargetRawYPos() => Svc.Targets.Target?.Position.Y ?? 0;
    public float GetTargetRawZPos() => Svc.Targets.Target?.Position.Z ?? 0;

    public float GetDistanceToPoint(float x, float y, float z) => Vector3.Distance(Svc.ClientState.LocalPlayer!.Position, new Vector3(x, y, z));
    public float GetDistanceToTarget() => Vector3.Distance(Svc.ClientState.LocalPlayer!.Position, Svc.Targets.Target?.Position ?? Svc.ClientState.LocalPlayer!.Position);
    public float GetDistanceToObject(string name) => Vector3.Distance(Svc.ClientState.LocalPlayer!.Position, Svc.Objects.First(x => x.Name.TextValue.Equals(name, System.StringComparison.InvariantCultureIgnoreCase)).Position);

    public unsafe bool IsTargetCasting() => ((Character*)Svc.Targets.Target?.Address!)->IsCasting;
    public unsafe uint GetTargetActionID() => ((Character*)Svc.Targets.Target?.Address!)->GetCastInfo()->ActionID;
    public unsafe uint GetTargetUsedActionID() => ((Character*)Svc.Targets.Target?.Address!)->GetCastInfo()->UsedActionId;
}
