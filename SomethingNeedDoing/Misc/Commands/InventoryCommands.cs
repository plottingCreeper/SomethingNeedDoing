using FFXIVClientStructs.FFXIV.Client.Game;

namespace SomethingNeedDoing.Misc.Commands;

public class InventoryCommands
{
    internal static InventoryCommands Instance { get; } = new();
    public unsafe int GetItemCount(int itemID, bool includeHQ = true) =>
       includeHQ ? InventoryManager.Instance()->GetInventoryItemCount((uint)itemID, true) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID)
       : InventoryManager.Instance()->GetInventoryItemCount((uint)itemID);

    public unsafe int GetInventoryFreeSlotCount()
    {
        InventoryType[] types = [InventoryType.Inventory1, InventoryType.Inventory2, InventoryType.Inventory3, InventoryType.Inventory4];
        var c = InventoryManager.Instance();
        var slots = 0;
        foreach (var x in types)
        {
            var inv = c->GetInventoryContainer(x);
            for (var i = 0; i < inv->Size; i++)
            {
                if (inv->Items[i].ItemID == 0)
                {
                    slots++;
                }
            }
        }
        return slots;
    }
}
