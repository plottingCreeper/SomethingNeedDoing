using AutoRetainerAPI;
using ECommons.DalamudServices;
using SomethingNeedDoing.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SomethingNeedDoing.Misc.Commands;

public class IpcCommands
{
    internal static IpcCommands Instance { get; } = new();

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

    private readonly AutoRetainerApi _autoRetainerApi;

    internal IpcCommands()
    {
        _autoRetainerApi = new();
        DeliverooIPC.Init();
    }

    public void Dispose()
    {
        _autoRetainerApi.Dispose();
        DeliverooIPC.Dispose();
    }

    public unsafe bool DeliverooIsTurnInRunning() => DeliverooIPC.IsTurnInRunning!.InvokeFunc();

    public unsafe bool ARAnyWaitingToBeProcessed(bool allCharacters = false) =>
        allCharacters ?
        ARRetainersWaitingToBeProcessed(allCharacters) || ARSubsWaitingToBeProcessed(allCharacters) :
        ARRetainersWaitingToBeProcessed() || ARSubsWaitingToBeProcessed();

    public unsafe bool ARRetainersWaitingToBeProcessed(bool allCharacters = false)
    {
        if (!allCharacters)
            return _autoRetainerApi.GetOfflineCharacterData(Svc.ClientState.LocalContentId).RetainerData.AsParallel().Any(x => x.HasVenture && x.VentureEndsAt <= DateTime.Now.ToUnixTimestamp());
        else
            return _autoRetainerApi.GetRegisteredCharacters().AsParallel().Any(character => _autoRetainerApi.GetOfflineCharacterData(character).RetainerData.Any(x => x.HasVenture && x.VentureEndsAt <= DateTime.Now.ToUnixTimestamp()));
    }

    public unsafe bool ARSubsWaitingToBeProcessed(bool allCharacters = false)
    {
        if (!allCharacters)
            return _autoRetainerApi.GetOfflineCharacterData(Svc.ClientState.LocalContentId).OfflineSubmarineData.AsParallel().Any(x => x.ReturnTime <= DateTime.Now.ToUnixTimestamp());
        else
            return _autoRetainerApi.GetRegisteredCharacters().AsParallel().Any(character => _autoRetainerApi.GetOfflineCharacterData(character).OfflineSubmarineData.Any(x => x.ReturnTime <= DateTime.Now.ToUnixTimestamp()));
    }
}
