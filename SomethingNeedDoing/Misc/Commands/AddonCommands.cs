using ECommons;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using SomethingNeedDoing.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SomethingNeedDoing.Misc.Commands;

public class AddonCommands
{
    internal static AddonCommands Instance { get; } = new();

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
    public unsafe bool IsAddonVisible(string addonName)
    {
        var ptr = Service.GameGui.GetAddonByName(addonName, 1);
        if (ptr == nint.Zero)
            return false;

        var addon = (AtkUnitBase*)ptr;
        return addon->IsVisible;
    }

    public unsafe bool IsNodeVisible(string addonName, int node, int child1 = -1, int child2 = -1)
    {
        var ptr = Service.GameGui.GetAddonByName(addonName, 1);
        if (ptr == nint.Zero)
            return false;

        var addon = (AtkUnitBase*)ptr;

        return child2 != -1
            ? addon->UldManager.NodeList[node]->ChildNode[child1].ChildNode[child2].IsVisible
            : child1 != -1
                ? addon->UldManager.NodeList[node]->ChildNode[child1].IsVisible
                : addon->UldManager.NodeList[node]->IsVisible;
    }

    public unsafe bool IsAddonReady(string addonName)
    {
        var ptr = Service.GameGui.GetAddonByName(addonName, 1);
        if (ptr == nint.Zero)
            return false;

        var addon = (AtkUnitBase*)ptr;
        return addon->UldManager.LoadedState == AtkLoadState.Loaded;
    }

    public unsafe string GetNodeText(string addonName, params int[] nodeNumbers)
    {
        if (nodeNumbers.Length == 0)
            throw new MacroCommandError("At least one node number is required");

        var ptr = Service.GameGui.GetAddonByName(addonName, 1);
        if (ptr == nint.Zero)
            throw new MacroCommandError($"Could not find {addonName} addon");

        var addon = (AtkUnitBase*)ptr;
        var uld = addon->UldManager;

        AtkResNode* node = null;
        var debugString = string.Empty;
        for (var i = 0; i < nodeNumbers.Length; i++)
        {
            var nodeNumber = nodeNumbers[i];

            var count = uld.NodeListCount;
            if (nodeNumber < 0 || nodeNumber >= count)
                throw new MacroCommandError($"Addon node number must be between 0 and {count} for the {addonName} addon");

            node = uld.NodeList[nodeNumber];
            debugString += $"[{nodeNumber}]";

            if (node == null)
                throw new MacroCommandError($"{addonName} addon node{debugString} is null");

            // More nodes to traverse
            if (i < nodeNumbers.Length - 1)
            {
                if ((int)node->Type < 1000)
                    throw new MacroCommandError($"{addonName} addon node{debugString} is not a component");

                uld = ((AtkComponentNode*)node)->Component->UldManager;
            }
        }

        if (node->Type != NodeType.Text)
            throw new MacroCommandError($"{addonName} addon node{debugString} is not a text node");

        var textNode = (AtkTextNode*)node;
        return textNode->NodeText.ToString();
    }

    public unsafe string GetSelectStringText(int index)
    {
        var ptr = Service.GameGui.GetAddonByName("SelectString", 1);
        if (ptr == nint.Zero)
            throw new MacroCommandError("Could not find SelectString addon");

        var addon = (AddonSelectString*)ptr;
        var popup = &addon->PopupMenu.PopupMenu;

        var count = popup->EntryCount;
        Service.Log.Debug($"index={index} // Count={count} // {index < 0 || index > count}");
        if (index < 0 || index > count)
            throw new MacroCommandError("Index out of range");

        var textPtr = popup->EntryNames[index];
        if (textPtr == null)
            throw new MacroCommandError("Text pointer was null");

        return Marshal.PtrToStringUTF8((nint)textPtr) ?? string.Empty;
    }

    public unsafe string GetSelectIconStringText(int index)
    {
        var ptr = Service.GameGui.GetAddonByName("SelectIconString", 1);
        if (ptr == nint.Zero)
            throw new MacroCommandError("Could not find SelectIconString addon");

        var addon = (AddonSelectIconString*)ptr;
        var popup = &addon->PopupMenu.PopupMenu;

        var count = popup->EntryCount;
        if (index < 0 || index > count)
            throw new MacroCommandError("Index out of range");

        var textPtr = popup->EntryNames[index];
        if (textPtr == null)
            throw new MacroCommandError("Text pointer was null");

        return Marshal.PtrToStringUTF8((nint)textPtr) ?? string.Empty;
    }

    public unsafe int GetNodeListCount(string addonName)
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>(addonName, out var addon))
        {
            return addon->UldManager.NodeListCount;
        }
        return 0;
    }
}
