using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Graphics.Environment;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;

namespace SomethingNeedDoing.Misc.Commands
{
    public class WorldStateCommands
    {
        internal static WorldStateCommands Instance { get; } = new();
        public int GetZoneID() => Service.ClientState.TerritoryType;

        public unsafe float GetFlagXCoord() => AgentMap.Instance()->FlagMapMarker.XFloat;
        public unsafe float GetFlagYCoord() => AgentMap.Instance()->FlagMapMarker.YFloat;

        public unsafe byte GetActiveWeatherID() => EnvManager.Instance()->ActiveWeather;
    }
}
