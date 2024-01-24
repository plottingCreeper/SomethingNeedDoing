using Dalamud.Utility;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Linq;

namespace SomethingNeedDoing.Misc.Commands;

public class CharacterStateCommands
{
    internal static CharacterStateCommands Instance { get; } = new();
    public unsafe bool HasStatus(string statusName)
    {
        statusName = statusName.ToLowerInvariant();
        var sheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>()!;
        var statusIDs = sheet
            .Where(row => row.Name.RawString.ToLowerInvariant() == statusName)
            .Select(row => row.RowId)
            .ToArray()!;

        return this.HasStatusId(statusIDs);
    }

    public unsafe bool HasStatusId(params uint[] statusIDs)
    {
        var statusID = Service.ClientState.LocalPlayer!.StatusList
            .Select(se => se.StatusId)
            .ToList().Intersect(statusIDs)
            .FirstOrDefault();

        return statusID != default;
    }

    public bool GetCharacterCondition(int flagID, bool hasCondition = true) => hasCondition ? Service.Condition[flagID] : !Service.Condition[flagID];

    public string GetCharacterName(bool includeWorld = false) =>
        Service.ClientState.LocalPlayer == null ? "null"
        : includeWorld ? $"{Service.ClientState.LocalPlayer.Name}@{Service.ClientState.LocalPlayer.HomeWorld.GameData!.Name}"
        : Service.ClientState.LocalPlayer.Name.ToString();

    public bool IsInZone(int zoneID) => Service.ClientState.TerritoryType == zoneID;

    

    public bool IsLocalPlayerNull() => Service.ClientState.LocalPlayer == null;

    public bool IsPlayerDead() => Service.ClientState.LocalPlayer!.IsDead;

    public bool IsPlayerCasting() => Service.ClientState.LocalPlayer!.IsCasting;

    public unsafe bool IsMoving() => AgentMap.Instance()->IsPlayerMoving == 1;

    public unsafe uint GetGil() => InventoryManager.Instance()->GetGil();

    public uint GetClassJobId() => Svc.ClientState.LocalPlayer!.ClassJob.Id;

    public float GetPlayerRawXPos(string character = "")
    {
        if (!character.IsNullOrEmpty())
        {
            unsafe
            {
                if (int.TryParse(character, out var p))
                {
                    var go = MiscHelpers.GetGameObjectFromPronounID((uint)(p + 42));
                    return go != null ? go->Position.X : -1;
                }
                else return Svc.Objects.Where(x => x.IsTargetable).FirstOrDefault(x => x.Name.ToString().Equals(character))?.Position.X ?? -1;
            }
        }
        return Svc.ClientState.LocalPlayer!.Position.X;
    }

    public float GetPlayerRawYPos(string character = "")
    {
        if (!character.IsNullOrEmpty())
        {
            unsafe
            {
                if (int.TryParse(character, out var p))
                {
                    var go = MiscHelpers.GetGameObjectFromPronounID((uint)(p + 42));
                    return go != null ? go->Position.Y : -1;
                }
                else return Svc.Objects.Where(x => x.IsTargetable).FirstOrDefault(x => x.Name.ToString().Equals(character))?.Position.Y ?? -1;
            }
        }
        return Svc.ClientState.LocalPlayer!.Position.Y;
    }

    public float GetPlayerRawZPos(string character = "")
    {
        if (!character.IsNullOrEmpty())
        {
            unsafe
            {
                if (int.TryParse(character, out var p))
                {
                    var go = MiscHelpers.GetGameObjectFromPronounID((uint)(p + 42));
                    return go != null ? go->Position.Z : -1;
                }
                else return Svc.Objects.Where(x => x.IsTargetable).FirstOrDefault(x => x.Name.ToString().Equals(character))?.Position.Z ?? -1;
            }
        }
        return Svc.ClientState.LocalPlayer!.Position.Z;
    }

    public unsafe int GetLevel(int expArrayIndex = -1)
    {
        if (expArrayIndex == -1) expArrayIndex = Svc.ClientState.LocalPlayer!.ClassJob.GameData!.ExpArrayIndex;
        return UIState.Instance()->PlayerState.ClassJobLevelArray[expArrayIndex];
    }

    public unsafe int GetFCRank() => ((InfoProxyFreeCompany*)Framework.Instance()->UIModule->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompany))->Rank;
}
