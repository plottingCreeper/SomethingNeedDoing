using System.Runtime.InteropServices;

namespace SomethingNeedDoing.Misc.Commands
{
    internal class ActionCommands
    {
        internal static ActionCommands Instance { get; } = new();

        private readonly AbandonDuty abandonDuty = Marshal.GetDelegateForFunctionPointer<AbandonDuty>(Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 43 28 B1 01"));

        private delegate void AbandonDuty(bool a1);

        public void LeaveDuty() => this.abandonDuty(false);
    }
}
