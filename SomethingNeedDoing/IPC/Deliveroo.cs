using Dalamud.Plugin.Ipc;

namespace SomethingNeedDoing.IPC;

internal static class DeliverooIPC
{
    private const string IsTurnInRunningStr = "Deliveroo.IsTurnInRunning";
    private const string TurnInStartedStr = "Deliveroo.TurnInStarted";
    private const string TurnInStoppedStr = "Deliveroo.TurnInStopped";

    internal static ICallGateSubscriber<bool>? IsTurnInRunning;
    internal static ICallGateSubscriber<object>? TurnInStarted;
    internal static ICallGateSubscriber<object>? TurnInStopped;

    internal static void Init()
    {
        IsTurnInRunning = Service.Interface.GetIpcSubscriber<bool>(IsTurnInRunningStr);
        TurnInStarted = Service.Interface.GetIpcSubscriber<object>(TurnInStartedStr);
        TurnInStopped = Service.Interface.GetIpcSubscriber<object>(TurnInStoppedStr);
    }

    internal static void Dispose()
    {
        IsTurnInRunning = null;
        TurnInStarted = null;
        TurnInStopped = null;
        IsTurnInRunning = null;
    }
}
