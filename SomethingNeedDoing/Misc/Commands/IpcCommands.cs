using AutoRetainerAPI;
using ECommons.DalamudServices;
using SomethingNeedDoing.IPC;
using System;
using System.Linq;

namespace SomethingNeedDoing.Misc;

/// <summary>
/// Miscellaneous functions that commands/scripts can use.
/// </summary>
public class IpcCommands
{
    private readonly AutoRetainerApi _autoRetainerApi;

    internal static IpcCommands Instance { get; } = new();

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
