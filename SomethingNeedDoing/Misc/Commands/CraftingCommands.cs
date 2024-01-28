using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using SomethingNeedDoing.Exceptions;

namespace SomethingNeedDoing.Misc.Commands;

public class CraftingCommands()
{
    internal static CraftingCommands Instance { get; } = new();

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
    public bool IsCrafting() => Service.Condition[ConditionFlag.Crafting] && !Service.Condition[ConditionFlag.PreparingToCraft];

    public bool IsNotCrafting() => !this.IsCrafting();

    private unsafe int GetNodeTextAsInt(AtkTextNode* node, string error)
    {
        try
        {
            if (node == null)
                throw new NullReferenceException("TextNode is null");

            var text = node->NodeText.ToString();
            var value = int.Parse(text);
            return value;
        }
        catch (Exception ex)
        {
            throw new MacroCommandError(error, ex);
        }
    }

    private unsafe AddonSynthesis* GetSynthesisAddon()
    {
        var ptr = Service.GameGui.GetAddonByName("Synthesis", 1);
        if (ptr == nint.Zero)
            throw new MacroCommandError("Could not find Synthesis addon");

        return (AddonSynthesis*)ptr;
    }

    public unsafe bool IsCollectable()
    {
        var addon = this.GetSynthesisAddon();
        return addon->AtkUnitBase.UldManager.NodeList[34]->IsVisible;
    }

    public unsafe string GetCondition(bool lower = true)
    {
        var addon = this.GetSynthesisAddon();
        var text = addon->Condition->NodeText.ToString();

        if (lower)
            text = text.ToLowerInvariant();

        return text;
    }

    public bool HasCondition(string condition, bool lower = true) => condition == this.GetCondition(lower);

    public unsafe int GetProgress()
    {
        var addon = this.GetSynthesisAddon();
        return this.GetNodeTextAsInt(addon->CurrentProgress, "Could not parse current progress number in the Synthesis addon");
    }

    public unsafe int GetMaxProgress()
    {
        var addon = this.GetSynthesisAddon();
        return this.GetNodeTextAsInt(addon->MaxProgress, "Could not parse max progress number in the Synthesis addon");
    }

    public bool HasMaxProgress()
    {
        var current = this.GetProgress();
        var max = this.GetMaxProgress();
        return current == max;
    }

    public unsafe int GetQuality()
    {
        var addon = this.GetSynthesisAddon();
        return this.GetNodeTextAsInt(addon->CurrentQuality, "Could not parse current quality number in the Synthesis addon");
    }

    public unsafe int GetMaxQuality()
    {
        var addon = this.GetSynthesisAddon();
        return this.GetNodeTextAsInt(addon->MaxQuality, "Could not parse max quality number in the Synthesis addon");
    }

    public bool HasMaxQuality()
    {
        var step = this.GetStep();

        if (step <= 1)
            return false;

        if (this.IsCollectable())
        {
            var current = this.GetQuality();
            var max = this.GetMaxQuality();
            return current == max;
        }
        else
        {
            var percentHq = this.GetPercentHQ();
            return percentHq == 100;
        }
    }

    public unsafe int GetDurability()
    {
        var addon = this.GetSynthesisAddon();
        return this.GetNodeTextAsInt(addon->CurrentDurability, "Could not parse current durability number in the Synthesis addon");
    }

    public unsafe int GetMaxDurability()
    {
        var addon = this.GetSynthesisAddon();
        return this.GetNodeTextAsInt(addon->StartingDurability, "Could not parse max durability number in the Synthesis addon");
    }

    public int GetCp()
    {
        var cp = Service.ClientState.LocalPlayer?.CurrentCp ?? 0;
        return (int)cp;
    }

    public int GetMaxCp()
    {
        var cp = Service.ClientState.LocalPlayer?.MaxCp ?? 0;
        return (int)cp;
    }

    public int GetGp()
    {
        var gp = Service.ClientState.LocalPlayer?.CurrentGp ?? 0;
        return (int)gp;
    }

    public int GetMaxGp()
    {
        var gp = Service.ClientState.LocalPlayer?.MaxGp ?? 0;
        return (int)gp;
    }

    public unsafe int GetStep()
    {
        var addon = this.GetSynthesisAddon();
        var step = this.GetNodeTextAsInt(addon->StepNumber, "Could not parse current step number in the Synthesis addon");
        return step;
    }

    public unsafe int GetPercentHQ()
    {
        var addon = this.GetSynthesisAddon();
        var step = this.GetNodeTextAsInt(addon->HQPercentage, "Could not parse percent hq number in the Synthesis addon");
        return step;
    }

    public unsafe bool NeedsRepair(float below = 0)
    {
        var im = InventoryManager.Instance();
        if (im == null)
        {
            Service.Log.Error("InventoryManager was null");
            return false;
        }

        var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null)
        {
            Service.Log.Error("InventoryContainer was null");
            return false;
        }

        if (equipped->Loaded == 0)
        {
            Service.Log.Error($"InventoryContainer is not loaded");
            return false;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
                continue;

            var itemCondition = Convert.ToInt32(Convert.ToDouble(item->Condition) / 30000.0 * 100.0);

            if (itemCondition <= below)
                return true;
        }

        return false;
    }

    public unsafe bool CanExtractMateria(float within = 100)
    {
        var im = InventoryManager.Instance();
        if (im == null)
        {
            Service.Log.Error("InventoryManager was null");
            return false;
        }

        var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null)
        {
            Service.Log.Error("InventoryContainer was null");
            return false;
        }

        if (equipped->Loaded == 0)
        {
            Service.Log.Error("InventoryContainer is not loaded");
            return false;
        }

        var nextHighest = 0f;
        var canExtract = false;
        var allExtract = true;
        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
                continue;

            var spiritbond = item->Spiritbond / 100;
            if (spiritbond == 100f)
            {
                canExtract = true;
            }
            else
            {
                allExtract = false;
                nextHighest = Math.Max(spiritbond, nextHighest);
            }
        }

        if (allExtract)
        {
            Service.Log.Debug("All items are spiritbound, pausing");
            return true;
        }

        if (canExtract)
        {
            // Don't wait, extract immediately
            if (within == 100)
            {
                Service.Log.Debug("An item is spiritbound, pausing");
                return true;
            }

            // Keep going if the next highest spiritbonded item is within the allowed range
            // i.e. 100 and 99, do another craft to finish the 99.
            if (nextHighest >= within)
            {
                Service.Log.Debug($"The next highest spiritbond is above ({nextHighest} >= {within}), keep going");
                return false;
            }
            else
            {
                Service.Log.Debug($"The next highest spiritbond is below ({nextHighest} < {within}), pausing");
                return true;
            }
        }

        return false;
    }

    public unsafe bool HasStats(uint craftsmanship, uint control, uint cp)
    {
        var uiState = UIState.Instance();
        if (uiState == null)
        {
            Service.Log.Error("UIState is null");
            return false;
        }

        var hasStats =
            uiState->PlayerState.Attributes[70] >= craftsmanship &&
            uiState->PlayerState.Attributes[71] >= control &&
            uiState->PlayerState.Attributes[11] >= cp;

        return hasStats;
    }

    public unsafe uint GetProgressIncrease(uint actionID) => this.GetActionResult(actionID).Progress;

    public unsafe uint GetQualityIncrease(uint actionID) => this.GetActionResult(actionID).Quality;

    private unsafe (uint Progress, uint Quality) GetActionResult(uint id)
    {

        var agent = AgentCraftActionSimulator.Instance();
        if (agent == null) return (0, 0);

        var progress = 0U;
        var quality = 0U;

        // Find Progress
        var p = (ProgressEfficiencyCalculation*)agent->Progress;
        for (var i = 0; i < sizeof(ProgressEfficiencyCalculations) / sizeof(ProgressEfficiencyCalculation); i++)
        {
            if (p == null) break;
            if (p->ActionId == id)
            {
                progress = p->ProgressIncrease;
                break;
            }

            p++;
        }

        var q = (QualityEfficiencyCalculation*)agent->Quality;
        for (var i = 0; i < sizeof(QualityEfficiencyCalculations) / sizeof(QualityEfficiencyCalculation); i++)
        {
            if (q == null) break;
            if (q->ActionId == id)
            {
                quality = q->QualityIncrease;
                break;
            }

            q++;
        }

        return (progress, quality);
    }
}
