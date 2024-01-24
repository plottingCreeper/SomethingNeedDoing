using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace SomethingNeedDoing.Misc
{
    internal class MiscHelpers
    {
        private static readonly unsafe nint pronounModule = (nint)Framework.Instance()->GetUiModule()->GetPronounModule();
        private static readonly unsafe delegate* unmanaged<nint, uint, GameObject*> getGameObjectFromPronounID = (delegate* unmanaged<nint, uint, GameObject*>)Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B D8 48 85 C0 0F 85 ?? ?? ?? ?? 8D 4F DD");
        public static unsafe GameObject* GetGameObjectFromPronounID(uint id) => getGameObjectFromPronounID(pronounModule, id);
    }
}
