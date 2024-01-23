//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using System.Threading;
//using System.Threading.Tasks;
//using ECommons.DalamudServices;
//using FFXIVClientStructs.FFXIV.Component.GUI;
//using SomethingNeedDoing.Grammar.Modifiers;
//using SomethingNeedDoing.Misc;
//using static ECommons.GenericHelpers;
//using ECommons.Automation;

//namespace SomethingNeedDoing.Grammar.Commands;

//internal class CallbackCommand : MacroCommand
//{
//    private static readonly Regex Regex = new(@"^/callback\s+(?<name>.*?)\s*$", RegexOptions.Compiled);
//    private readonly unsafe AtkUnitBase* addon;
//    private readonly bool updateState;
//    private readonly List<object> valueArgs = [];

//    private unsafe CallbackCommand(AtkUnitBase* addon, bool updateState, List<object> valueArgs, WaitModifier wait) : base("", wait)
//    {
//        this.addon = addon;
//        this.updateState = updateState;
//        this.valueArgs = valueArgs;
//    }

//    public unsafe static CallbackCommand Parse(List<string> args)
//    {
//        var text = string.Join(" ", args);
//        _ = WaitModifier.TryParse(ref text, out var waitModifier);


//        if (!TryGetAddonByName<AtkUnitBase>(args[0], out var addonArg))
//        {
//            Svc.Log.Info($"Invalid addon {args[0]}. Please follow \"callback <addon> <bool> <atkValues>\"");
//        }
//        if (!bool.TryParse(args[1], out var boolArg))
//        {
//            Svc.Log.Info($"Invalid bool. Please follow \"callback <addon> <bool> <atkValues>\"");
//        }

//        var valueArgs = new List<object>();

//        var current = "";
//        var inQuotes = false;

//        for (var i = 0; i < args.Count; i++)
//        {
//            if (!inQuotes)
//            {
//                if (args[i].StartsWith("\""))
//                {
//                    inQuotes = true;
//                    current = args[i].TrimStart('"');
//                }
//                else
//                {
//                    if (int.TryParse(args[i], out var iValue)) valueArgs.Add(iValue);
//                    else if (uint.TryParse(args[i].TrimEnd('U', 'u'), out var uValue)) valueArgs.Add(uValue);
//                    else if (bool.TryParse(args[i], out var bValue)) valueArgs.Add(bValue);
//                    else valueArgs.Add(args[i]);
//                }
//            }
//            else
//            {
//                if (args[i].EndsWith("\""))
//                {
//                    inQuotes = false;
//                    current += " " + args[i].TrimEnd('"');
//                    valueArgs.Add(current);
//                    current = "";
//                }
//                else
//                {
//                    current += " " + args[i];
//                }
//            }
//        }

//        if (!string.IsNullOrEmpty(current))
//        {
//            Svc.Log.Error("Error: Unclosed quotes.");
//        }

//        return new CallbackCommand(addonArg, boolArg, valueArgs, waitModifier);
//    }

//    public async override Task Execute(ActiveMacro macro, CancellationToken token)
//    {
//        unsafe
//        {
//            Callback.Fire(addon, updateState, valueArgs.ToArray());
//        }
//        await this.PerformWait(token);
//    }
//}
