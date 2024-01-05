using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace SomethingNeedDoing.Interface;

/// <summary>
/// Help window for macro creation.
/// </summary>
internal class HelpWindow : Window
{
    private static readonly Vector4 ShadedColor = new(0.68f, 0.68f, 0.68f, 1.0f);

    private readonly (string Name, string? Alias, string Description, string[] Modifiers, string[] Examples)[] commandData = new[]
    {
        (
            "action", "ac",
            "Execute an action and wait for the server to respond.",
            new[] { "wait", "unsafe", "condition" },
            new[]
            {
                "/ac Groundwork",
                "/ac \"Tricks of the Trade\"",
            }),
        (
            "click", null,
            "Click a pre-defined button in an addon or window.",
            new[] { "wait" },
            new[]
            {
                "/click synthesize",
            }),
        (
            "craft", "gate",
            "Similar to loop but used at the start of a macro with an infinite /loop at the end. Allows a certain amount of executions before stopping the macro.",
            new[] { "echo", "wait" },
            new[]
            {
                "/craft 10",
            }),
        (
            "loop", null,
            "Loop the current macro forever, or a certain amount of times.",
            new[] { "wait", "echo" },
            new[]
            {
                "/loop",
                "/loop 5",
            }),
        (
            "recipe", null,
            "Open the recipe book to a specific recipe.",
            new[] { "wait" },
            new[]
            {
                "/recipe \"Tsai tou Vounou\"",
            }),
        (
            "require", null,
            "Require a certain effect to be present before continuing.",
            new[] { "wait", "maxwait" },
            new[]
            {
                "/require \"Well Fed\"",
            }),
        (
            "requirequality", null,
            "Require a certain amount of quality be present before continuing.",
            new[] { "wait", "maxwait" },
            new[]
            {
                "/requirequality 3000",
            }),
        (
            "requirerepair", null,
            "Pause if an item is at zero durability.",
            new[] { "wait" },
            new[]
            {
                "/requirerepair",
            }),
        (
            "requirespiritbond", null,
            "Pause when an item is ready to have materia extracted. Optional argument to keep crafting if the next highest spiritbond is greater-than-or-equal to the argument value.",
            new[] { "wait" },
            new[]
            {
                "/requirespiritbond",
                "/requirespiritbond 99.5",
            }),
        (
            "requirestats", null,
            "Require a certain amount of stats effect to be present before continuing. Syntax is Craftsmanship, Control, then CP.",
            new[] { "wait", "maxwait" },
            new[]
            {
                "/requirestats 2700 2600 500",
            }),
        (
            "item", null,
            "Use an item, stopping the macro if the item is not present.",
            new[] { "hq", "wait" },
            new[]
            {
                "/item Calamari Ripieni",
                "/item Calamari Ripieni <hq> <wait.3>",
            }),
        (
            "runmacro", null,
            "Start a macro from within another macro.",
            new[] { "wait" },
            new[]
            {
                "/runmacro \"Sub macro\"",
            }),
        (
            "send", null,
            "Send an arbitrary keystroke with optional modifiers. Keys are pressed in the same order as the command.",
            new[] { "wait" },
            new[]
            {
                "/send MULTIPLY",
                "/send NUMPAD0",
                "/send CONTROL+MENU+SHIFT+NUMPAD0",
            }),
        (
            "hold", null,
            "Send an arbitrary keystroke, to be held down, with optional modifiers. Keys are pressed in the same order as the command.",
            new[] { "wait" },
            new[]
            {
                "/hold MULTIPLY",
                "/hold NUMPAD0",
                "/hold CONTROL+MENU+SHIFT+NUMPAD0",
            }),
        (
            "release", null,
            "Send an arbitrary keystroke, to be released, with optional modifiers. Keys are pressed in the same order as the command.",
            new[] { "wait" },
            new[]
            {
                "/release MULTIPLY",
                "/release NUMPAD0",
                "/release CONTROL+MENU+SHIFT+NUMPAD0",
            }),
        (
            "target", null,
            "Target anyone and anything that can be selected.",
            new[] { "wait", "index" },
            new[]
            {
                "/target Eirikur",
                "/target Moyce",
            }),
        (
            "waitaddon", null,
            "Wait for an addon, otherwise known as a UI component to be present. You can discover these names by using the \"Addon Inspector\" view inside the \"/xldata\" window.",
            new[] { "wait", "maxwait" },
            new[]
            {
                "/waitaddon RecipeNote",
            }),
        (
            "wait", null,
            "The same as the wait modifier, but as a command.",
            Array.Empty<string>(),
            new[]
            {
                "/wait 1-5",
            }),
    };

    private readonly (string Name, string Description, string[] Examples)[] modifierData = new[]
    {
        (
            "wait",
            "Wait a certain amount of time, or a random time within a range.",
            new[]
            {
                "/ac Groundwork <wait.3>       # Wait 3 seconds",
                "/ac Groundwork <wait.3.5>     # Wait 3.5 seconds",
                "/ac Groundwork <wait.1-5>     # Wait between 1 and 5 seconds",
                "/ac Groundwork <wait.1.5-5.5> # Wait between 1.5 and 5.5 seconds",
            }),
        (
            "maxwait",
            "For certain commands, the maximum time to wait for a certain state to be achieved. By default, this is 5 seconds.",
            new[]
            {
                "/waitaddon RecipeNote <maxwait.10>",
            }),
        (
            "condition",
            "Require a crafting condition to perform the action specified. This is taken from the Synthesis window and may be localized to your client language.",
            new[]
            {
                "/ac Observe <condition.poor>",
                "/ac \"Precise Touch\" <condition.good,excellent>",
                "/ac \"Byregot's Blessing\" <condition.not.poor>",
                "/ac \"Byregot's Blessing\" <condition.!poor>",
            }),
        (
            "unsafe",
            "Prevent the /action command from waiting for a positive server response and attempting to execute the command anyways.",
            new[]
            {
                "/ac \"Tricks of the Trade\" <unsafe>",
            }),
        (
            "echo",
            "Echo the amount of loops remaining after executing a /loop command.",
            new[]
            {
                "/loop 5 <echo>",
            }),
        (
            "index",
            "For supported commands, specify the index. For example, when there are multiple targets with the same name.",
            new[]
            {
                "/target abc <index.5>",
            }),
    };

    private readonly (string Name, string Description, string? Example)[] cliData = new[]
    {
        ("help", "Show this window.", null),
        ("run", "Run a macro, the name must be unique.", "/pcraft run MyMacro"),
        ("run loop #", "Run a macro and then loop N times, the name must be unique. Only the last /loop in the macro is replaced", "/pcraft run loop 5 MyMacro"),
        ("pause", "Pause the currently executing macro.", null),
        ("pause loop", "Pause the currently executing macro at the next /loop.", null),
        ("resume", "Resume the currently paused macro.", null),
        ("stop", "Clear the currently executing macro list.", null),
        ("stop loop", "Clear the currently executing macro list at the next /loop.", null),
    };

    private readonly List<string> clickNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpWindow"/> class.
    /// </summary>
    public HelpWindow()
        : base("Something Need Doing Help")
    {
        this.Flags |= ImGuiWindowFlags.NoScrollbar;

        this.Size = new Vector2(400, 600);
        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.RespectCloseHotkey = false;

        this.clickNames = ClickLib.Click.GetClickNames();
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        if (ImGui.BeginTabBar("HelpTab"))
        {
            var tabs = new (string Title, System.Action Dele)[]
            {
                ("Changelog", this.DrawChangelog),
                ("Options", this.DrawOptions),
                ("Commands", this.DrawCommands),
                ("Modifiers", this.DrawModifiers),
                ("Lua", this.DrawLua),
                ("CLI", this.DrawCli),
                ("Clicks", this.DrawClicks),
                ("Sends", this.DrawVirtualKeys),
                ("Conditions", this.DrawAllConditions),
                ("Game Data", this.DrawGameData),
            };

            foreach (var (title, dele) in tabs)
            {
                if (ImGui.BeginTabItem(title))
                {
                    ImGui.BeginChild("scrolling", new Vector2(0, -1), false);

                    dele();

                    ImGui.EndChild();

                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();
        }

        ImGui.EndChild();
    }

    private void DrawChangelog()
    {
        static void DisplayChangelog(string date, string changes, bool separator = true)
        {
            ImGui.Text(date);
            ImGui.PushStyleColor(ImGuiCol.Text, ShadedColor);
            ImGui.TextWrapped(changes);
            ImGui.PopStyleColor();

            if (separator)
                ImGui.Separator();
        }

        ImGui.PushFont(UiBuilder.MonoFont);

        DisplayChangelog(
          "2024-01-04",
          "- Added IsNodeVisible().\n");

        DisplayChangelog(
           "2023-12-22",
           "- Updated the GetRaw coordinate functions to take in an object name or party member position.\n");

        DisplayChangelog(
           "2023-12-12",
           "- Added IsQuestAccepted()\n" +
           "- Added IsQuestComplete()\n" +
           "- Added GetQuestSequence()\n" +
           "- Added GetQuestIDByName()\n" +
           "- Added GetQuestNameByID()\n" +
           "- Added GetNodeListCount()\n" +
           "- Added GetTargetName()\n");

        DisplayChangelog(
           "2023-11-06",
           "- Added GetLevel()\n" +
           "- Added \"Game Data\" tab to the help menu.\n" +
           "- Added GetGp() and GetMaxGp() (thanks nihilistzsche)\n" +
           "- Added GetFCRank()\n");

        DisplayChangelog(
           "2023-11-23",
           "- Added GetPlayerRawXPos()\n" +
           "- Added GetPlayerRawZPos()\n" +
           "- Added GetPlayerRawYPos()\n" +
           "- Added GetDistanceToPoint()\n");

        DisplayChangelog(
           "2023-11-23",
           "- Fix for IsMoving() to detect forms of automated movement.\n");

        DisplayChangelog(
           "2023-11-21",
           "- Added IsMoving()\n");

        DisplayChangelog(
           "2023-11-20",
           "- Macros will now automatically prefix non-command text lines with /echo. To send a message to chat now requires you to prefix it with the appropiate chat channel command.\n");

        DisplayChangelog(
           "2023-11-17",
           "- Added /hold\n" +
           "- Added /release.\n" +
           "- Updated help documentation for lua commands.\n");

        DisplayChangelog(
           "2023-11-15",
           "- Added GetClassJobId()\n");

        DisplayChangelog(
           "2023-11-14",
           "- Fixed the targeting system to ignore untargetable objects.\n" +
           "- Fixed the targeting system to prefer closest matches.\n" +
           "- Added an option to not use SND's targeting system.\n" +
           "- Added an option to not stop the macro if a target is not found.\n");

        DisplayChangelog(
           "2023-11-11",
           "- The main command is now /somethingneeddoing. The aliases are /snd and /pcraft.\n" +
           "- Changed how the /send command works internally for compatibility with XIVAlexander.\n");

        DisplayChangelog(
           "2023-11-08",
           "- Added GetGil()\n");

        DisplayChangelog(
           "2023-11-06",
           "- Added IsLocalPlayerNull()\n" +
           "- Added IsPlayerDead()\n" +
           "- Added IsPlayerCasting()\n");

        DisplayChangelog(
           "2023-11-05",
           "- Added LeaveDuty().\n");

        DisplayChangelog(
           "2023-11-04",
           "- Added GetProgressIncrease(uint actionID). Returns numerical amount of progress increase a given action will cause.\n" +
           "- Added GetQualityIncrease(uint actionID). Returns numerical amount of quality increase a given action will cause.\n");

        DisplayChangelog(
           "2023-10-24",
           "- Changed GetCharacterCondition() to take in an int instead of a string.\n" +
           "- Added a list of conditions to the help menu.\n");

        DisplayChangelog(
           "2023-10-21",
           "- Added an optional bool to pass to GetCharacterName to return the world name in addition.\n");

        DisplayChangelog(
           "2023-10-20",
           "- Changed GetItemCount() to support HQ items. Default behaviour includes both HQ and NQ. Pass false to the function to do only NQ.\n");

        DisplayChangelog(
            "2023-10-17",
            "- Added a Deliveroo IPC along with the DeliverooIsTurnInRunning() lua command.\n");

        DisplayChangelog(
            "2023-10-13",
            "- Added a small delay to /loop so that very short looping macros will not crash the client.\n" +
            "- Added a lock icon to the window bar to the lock the window position.\n");

        DisplayChangelog(
            "2023-10-10",
            "- Added IsInZone() lua command. Pass the zoneID, returns a bool.\n" +
            "- Added GetZoneID() lua command. Gets the zoneID of the current zone.\n" +
            "- Added GetCharacterName() lua command.\n" +
            "- Added GetItemCount() lua command. Pass the itemID, get count.\n");

        DisplayChangelog(
            "2023-05-31",
            "- Added the index modifier\n");

        DisplayChangelog(
            "2022-08-22",
            "- Added use item command.\n" +
            "- Updated Lua method GetNodeText to get nested nodes.\n");

        DisplayChangelog(
            "2022-07-23",
            "- Fixed Lua methods (oops).\n" +
            "- Add Lua methods to get SelectString and SelectIconString text.\n");

        DisplayChangelog(
            "2022-06-10",
            "- Updated the Send command to allow for '+' delimited modifiers.\n" +
            "- Added a CraftLoop template feature to allow for customization of the loop capability.\n" +
            "- Added an option to customize the error/notification beeps.\n" +
            "- Added Lua scripting available as a button next to the CraftLoop buttons.\n" +
            "- Updated the help window options tab to use collapsing headers.\n");

        DisplayChangelog(
            "2022-05-13",
            "- Added a /requirequality command to require a certain amount of quality before synthesizing.\n" +
            "- Added a /requirerepair command to pause when an equipped item is broken.\n" +
            "- Added a /requirespiritbond command to pause when an item can have materia extracted.");

        DisplayChangelog(
            "2022-04-26",
            "- Added a max retries option for when an action command does not receive a response within the alloted limit, typically due to lag.\n" +
            "- Added a noisy errors option to play some beeps when a detectable error occurs.");

        DisplayChangelog(
            "2022-04-25",
            "- Added a /recipe command to open the recipe book to a specific recipe (ty marimelon).\n");

        DisplayChangelog(
            "2022-04-18",
            "- Added a /craft command to act as a gate at the start of a macro, rather than specifying the number of loops at the end.\n" +
            "- Removed the \"Loop Total\" option, use the /craft or /gate command instead of this jank.");

        DisplayChangelog(
            "2022-04-04",
            "- Added macro CraftLoop loop UI options to remove /loop boilerplate (ty darkarchon).\n");

        DisplayChangelog(
            "2022-04-03",
            "- Fixed condition modifier to work with non-English letters/characters.\n" +
            "- Added an option to disable monospaced font for JP users.\n");

        DisplayChangelog(
            "2022-03-03",
            "- Added an intelligent wait option that waits until your crafting action is complete, rather than what is in the <wait> modifier.\n" +
            "- Updated the <condition> modifier to accept a comma delimited list of names.\n");

        DisplayChangelog(
            "2022-02-02",
            "- Added /send help pane.\n" +
            "- Fixed /loop echo commands not being sent to the echo channel.\n");

        DisplayChangelog(
            "2022-01-30",
            "- Added a \"Step\" button to the control bar that lets you skip to the next step when a macro is paused.\n");

        DisplayChangelog(
            "2022-01-25",
            "- The help menu now has an options pane.\n" +
            "- Added an option to disable skipping craft actions when not crafting or at max progress.\n" +
            "- Added an option to disable the automatic quality increasing action skip, when at max quality.\n" +
            "- Added an option to treat /loop as the total iterations, rather than the amount to repeat.\n" +
            "- Added an option to always treat /loop commands as having an <echo> modifier.\n");

        DisplayChangelog(
            "2022-01-16",
            "- The help menu now has a /click listing.\n" +
            "- Various quality increasing skills are skipped when at max quality. Please open an issue if you encounter issues with this.\n" +
            "- /loop # will reset after reaching the desired amount of loops. This allows for nested looping. You can test this with the following:\n" +
            "    /echo 111 <wait.1>\n" +
            "    /loop 1\n" +
            "    /echo 222 <wait.1>\n" +
            "    /loop 1\n" +
            "    /echo 333 <wait.1>\n");

        DisplayChangelog(
            "2022-01-01",
            "- Various /pcraft commands have been added. View the help menu for more details.\n" +
            "- There is also a help menu.\n",
            false);

        ImGui.PopFont();
    }

    private void DrawOptions()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        static void DisplayOption(params string[] lines)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ShadedColor);

            foreach (var line in lines)
                ImGui.TextWrapped(line);

            ImGui.PopStyleColor();
        }

        if (ImGui.CollapsingHeader("Crafting skips"))
        {
            var craftSkip = Service.Configuration.CraftSkip;
            if (ImGui.Checkbox("Craft Skip", ref craftSkip))
            {
                Service.Configuration.CraftSkip = craftSkip;
                Service.Configuration.Save();
            }

            DisplayOption("- Skip craft actions when not crafting.");

            ImGui.Separator();

            var smartWait = Service.Configuration.SmartWait;
            if (ImGui.Checkbox("Smart Wait", ref smartWait))
            {
                Service.Configuration.SmartWait = smartWait;
                Service.Configuration.Save();
            }

            DisplayOption("- Intelligently wait for crafting actions to complete instead of using the <wait> or <unsafe> modifiers.");

            ImGui.Separator();

            var qualitySkip = Service.Configuration.QualitySkip;
            if (ImGui.Checkbox("Quality Skip", ref qualitySkip))
            {
                Service.Configuration.QualitySkip = qualitySkip;
                Service.Configuration.Save();
            }

            DisplayOption("- Skip quality increasing actions when the HQ chance is at 100%%. If you depend on durability increases from Manipulation towards the end of your macro, you will likely want to disable this.");
        }

        if (ImGui.CollapsingHeader("Loop echo"))
        {
            var loopEcho = Service.Configuration.LoopEcho;
            if (ImGui.Checkbox("Craft and Loop Echo", ref loopEcho))
            {
                Service.Configuration.LoopEcho = loopEcho;
                Service.Configuration.Save();
            }

            DisplayOption("- /loop and /craft commands will always have an <echo> tag applied.");
        }

        if (ImGui.CollapsingHeader("Action retry"))
        {
            ImGui.SetNextItemWidth(50);
            var maxTimeoutRetries = Service.Configuration.MaxTimeoutRetries;
            if (ImGui.InputInt("Action max timeout retries", ref maxTimeoutRetries, 0))
            {
                if (maxTimeoutRetries < 0)
                    maxTimeoutRetries = 0;
                if (maxTimeoutRetries > 10)
                    maxTimeoutRetries = 10;

                Service.Configuration.MaxTimeoutRetries = maxTimeoutRetries;
                Service.Configuration.Save();
            }

            DisplayOption("- The number of times to re-attempt an action command when a timely response is not received.");
        }

        if (ImGui.CollapsingHeader("Font"))
        {
            var disableMonospaced = Service.Configuration.DisableMonospaced;
            if (ImGui.Checkbox("Disable Monospaced fonts", ref disableMonospaced))
            {
                Service.Configuration.DisableMonospaced = disableMonospaced;
                Service.Configuration.Save();
            }

            DisplayOption("- Use the regular font instead of monospaced in the macro window. This may be handy for JP users so as to prevent missing unicode errors.");
        }

        if (ImGui.CollapsingHeader("Craft loop"))
        {
            var useCraftLoopTemplate = Service.Configuration.UseCraftLoopTemplate;
            if (ImGui.Checkbox("Enable CraftLoop templating", ref useCraftLoopTemplate))
            {
                Service.Configuration.UseCraftLoopTemplate = useCraftLoopTemplate;
                Service.Configuration.Save();
            }

            DisplayOption($"- When enabled the CraftLoop template will replace various placeholders with values.");

            if (useCraftLoopTemplate)
            {
                var craftLoopTemplate = Service.Configuration.CraftLoopTemplate;

                const string macroKeyword = "{{macro}}";
                const string countKeyword = "{{count}}";

                if (!craftLoopTemplate.Contains(macroKeyword))
                    ImGui.TextColored(ImGuiColors.DPSRed, $"{macroKeyword} must be present in the template");

                DisplayOption($"- {macroKeyword} inserts the current macro content.");
                DisplayOption($"- {countKeyword} inserts the loop count for various commands.");

                if (ImGui.InputTextMultiline("CraftLoopTemplate", ref craftLoopTemplate, 100_000, new Vector2(-1, 200)))
                {
                    Service.Configuration.CraftLoopTemplate = craftLoopTemplate;
                    Service.Configuration.Save();
                }
            }
            else
            {
                var craftLoopFromRecipeNote = Service.Configuration.CraftLoopFromRecipeNote;
                if (ImGui.Checkbox("CraftLoop starts in the Crafting Log", ref craftLoopFromRecipeNote))
                {
                    Service.Configuration.CraftLoopFromRecipeNote = craftLoopFromRecipeNote;
                    Service.Configuration.Save();
                }

                DisplayOption("- When enabled the CraftLoop option will expect the Crafting Log to be visible, otherwise the Synthesis window must be visible.");

                var craftLoopEcho = Service.Configuration.CraftLoopEcho;
                if (ImGui.Checkbox("CraftLoop Craft and Loop echo", ref craftLoopEcho))
                {
                    Service.Configuration.CraftLoopEcho = craftLoopEcho;
                    Service.Configuration.Save();
                }

                DisplayOption("- When enabled the /craft or /gate commands supplied by the CraftLoop option will have an echo modifier.");

                ImGui.SetNextItemWidth(50);
                var craftLoopMaxWait = Service.Configuration.CraftLoopMaxWait;
                if (ImGui.InputInt("CraftLoop maxwait", ref craftLoopMaxWait, 0))
                {
                    if (craftLoopMaxWait < 0)
                        craftLoopMaxWait = 0;

                    if (craftLoopMaxWait != Service.Configuration.CraftLoopMaxWait)
                    {
                        Service.Configuration.CraftLoopMaxWait = craftLoopMaxWait;
                        Service.Configuration.Save();
                    }
                }

                DisplayOption("- The CraftLoop /waitaddon \"...\" <maxwait> modifiers have their maximum wait set to this value.");
            }
        }

        if (ImGui.CollapsingHeader("Chat"))
        {
            var names = Enum.GetNames<XivChatType>();
            var chatTypes = Enum.GetValues<XivChatType>();

            var current = Array.IndexOf(chatTypes, Service.Configuration.ChatType);
            if (current == -1)
            {
                current = Array.IndexOf(chatTypes, Service.Configuration.ChatType = XivChatType.Echo);
                Service.Configuration.Save();
            }

            ImGui.SetNextItemWidth(200f);
            if (ImGui.Combo("Normal chat channel", ref current, names, names.Length))
            {
                Service.Configuration.ChatType = chatTypes[current];
                Service.Configuration.Save();
            }

            var currentError = Array.IndexOf(chatTypes, Service.Configuration.ErrorChatType);
            if (currentError == -1)
            {
                currentError = Array.IndexOf(chatTypes, Service.Configuration.ErrorChatType = XivChatType.Urgent);
                Service.Configuration.Save();
            }

            ImGui.SetNextItemWidth(200f);
            if (ImGui.Combo("Error chat channel", ref currentError, names, names.Length))
            {
                Service.Configuration.ChatType = chatTypes[currentError];
                Service.Configuration.Save();
            }
        }

        if (ImGui.CollapsingHeader("Error beeps"))
        {
            var noisyErrors = Service.Configuration.NoisyErrors;
            if (ImGui.Checkbox("Noisy errors", ref noisyErrors))
            {
                Service.Configuration.NoisyErrors = noisyErrors;
                Service.Configuration.Save();
            }

            DisplayOption("- When a check fails or error happens, some helpful beeps will play to get your attention.");

            ImGui.SetNextItemWidth(50f);
            var beepFrequency = Service.Configuration.BeepFrequency;
            if (ImGui.InputInt("Beep frequency", ref beepFrequency, 0))
            {
                Service.Configuration.BeepFrequency = beepFrequency;
                Service.Configuration.Save();
            }

            ImGui.SetNextItemWidth(50f);
            var beepDuration = Service.Configuration.BeepDuration;
            if (ImGui.InputInt("Beep duration", ref beepDuration, 0))
            {
                Service.Configuration.BeepDuration = beepDuration;
                Service.Configuration.Save();
            }

            ImGui.SetNextItemWidth(50f);
            var beepCount = Service.Configuration.BeepCount;
            if (ImGui.InputInt("Beep count", ref beepCount, 0))
            {
                Service.Configuration.BeepCount = beepCount;
                Service.Configuration.Save();
            }

            if (ImGui.Button("Beep test"))
            {
                Task.Run(() =>
                {
                    for (var i = 0; i < beepCount; i++)
                        Console.Beep(beepFrequency, beepDuration);
                });
            }
        }

        if (ImGui.CollapsingHeader("/target"))
        {
            var defaultTarget = Service.Configuration.UseSNDTargeting;
            if (ImGui.Checkbox("Use SND's targeting system.", ref defaultTarget))
            {
                Service.Configuration.UseSNDTargeting = defaultTarget;
                Service.Configuration.Save();
            }

            var stopMacroIfNoTarget = Service.Configuration.StopMacroIfTargetNotFound;
            if (ImGui.Checkbox("Stop macro if target not found (only applies to SND's targeting system).", ref stopMacroIfNoTarget))
            {
                Service.Configuration.StopMacroIfTargetNotFound = stopMacroIfNoTarget;
                Service.Configuration.Save();
            }

            DisplayOption("- Override the behaviour of /target with SND's system.");
        }

        ImGui.PopFont();
    }

    private void DrawCommands()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        foreach (var (name, alias, desc, modifiers, examples) in this.commandData)
        {
            ImGui.Text($"/{name}");

            ImGui.PushStyleColor(ImGuiCol.Text, ShadedColor);

            if (alias != null)
                ImGui.Text($"- Alias: /{alias}");

            ImGui.TextWrapped($"- Description: {desc}");

            ImGui.Text("- Modifiers:");
            foreach (var mod in modifiers)
                ImGui.Text($"  - <{mod}>");

            ImGui.Text("- Examples:");
            foreach (var example in examples)
                ImGui.Text($"  - {example}");

            ImGui.PopStyleColor();

            ImGui.Separator();
        }

        ImGui.PopFont();
    }

    private void DrawModifiers()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        foreach (var (name, desc, examples) in this.modifierData)
        {
            ImGui.Text($"<{name}>");

            ImGui.PushStyleColor(ImGuiCol.Text, ShadedColor);

            ImGui.TextWrapped($"- Description: {desc}");

            ImGui.Text("- Examples:");
            foreach (var example in examples)
                ImGui.Text($"  - {example}");

            ImGui.PopStyleColor();

            ImGui.Separator();
        }

        ImGui.PopFont();
    }

    private void DrawCli()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        foreach (var (name, desc, example) in this.cliData)
        {
            ImGui.Text($"/pcraft {name}");

            ImGui.PushStyleColor(ImGuiCol.Text, ShadedColor);

            ImGui.TextWrapped($"- Description: {desc}");

            if (example != null)
            {
                ImGui.Text($"- Example: {example}");
            }

            ImGui.PopStyleColor();

            ImGui.Separator();
        }

        ImGui.PopFont();
    }

    private void DrawLua()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        var text = @"
Lua scripts work by yielding commands back to the macro engine.

For example:

yield(""/ac Muscle memory <wait.3>"")
yield(""/ac Precise touch <wait.2>"")
yield(""/echo done!"")
...and so on.

Documentation for these functions are available at:
https://github.com/daemitus/SomethingNeedDoing/blob/master/SomethingNeedDoing/Misc/ICommandInterface.cs

===Available functions===
bool IsCrafting()
bool IsNotCrafting()
bool IsCollectable()

// lower: Get the condition in lowercase
string GetCondition(bool lower = true)

// condition: The condition name, as displayed in the UI
// lower:     Get the condition in lowercase
bool HasCondition(string condition, bool lower = true)

int GetProgress()
int GetMaxProgress()
bool HasMaxProgress()

int GetQuality()
int GetMaxQuality()
bool HasMaxQuality()

int GetDurability()
int GetMaxDurability()

int GetCp()
int GetMaxCp()

int GetStep()
int GetPercentHQ()
bool NeedsRepair()

// within: Return false if the next highest spiritbond is >= the within value.
bool CanExtractMateria(float within = 100)

bool HasStats(uint craftsmanship, uint control, uint cp)

// name: status effect name
bool HasStatus(string name)

// id: status effect id(s).
bool HasStatusId(uint id, ...)

bool IsAddonVisible(string addonName)
bool IsNodeVisible(string addonName, int node)
bool IsAddonReady(string addonName)

// Can fetch nested nodes
string GetNodeText(string addonName, int nodeNumber, ...)

string GetSelectStringText(int index)
string GetSelectIconStringText(int index)

bool GetCharacterCondition(int flagID, bool hasCondition = true)

bool IsInZone(int zoneID)
int GetZoneID()

string GetCharacterName(bool includeWorld = false)

int GetItemCount(int itemID, bool includeHQ = true)

bool DeliverooIsTurnInRunning()

uint GetProgressIncrease(uint actionID)
uint GetQualityIncrease(uint actionID)

void LeaveDuty()

bool IsLocalPlayerNull()
bool IsPlayerDead()
bool IsPlayerCasting()
bool IsMoving()

uint GetGil()

uint GetClassJobId()

// if you pass an invalid name or party position it will return -1
float GetPlayerRawXPos(string character = "")
float GetPlayerRawYPos(string character = "")
float GetPlayerRawZPos(string character = "")
float GetDistanceToPoint(float x, float y, float z))

int GetLevel(uint ExpArrayIndex = -1)

string GetQuestNameByID(ushort id)
bool IsQuestAccepted(ushort id)
bool IsQuestComplete(ushort id)
byte GetQuestSequence(ushort id)
uint? GetQuestIDByName(string name)

int GetNodeListCount(string addonName)

string GetTargetName()
".Trim();

        ImGui.TextWrapped(text);

        ImGui.PopFont();
    }

    private void DrawClicks()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        ImGui.TextWrapped("Refer to https://github.com/daemitus/ClickLib/tree/master/ClickLib/Clicks for any details.");
        ImGui.Separator();

        foreach (var name in this.clickNames)
        {
            ImGui.Text($"/click {name}");
        }

        ImGui.PopFont();
    }

    private void DrawVirtualKeys()
    {
        ImGui.PushFont(UiBuilder.MonoFont);

        ImGui.TextWrapped("Active keys will highlight green.");
        ImGui.Separator();

        var validKeys = Service.KeyState.GetValidVirtualKeys().ToHashSet();

        var names = Enum.GetNames<VirtualKey>();
        var values = Enum.GetValues<VirtualKey>();

        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var vkCode = values[i];

            if (!validKeys.Contains(vkCode))
                continue;

            var isActive = Service.KeyState[vkCode];

            if (isActive)
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);

            ImGui.Text($"/send {name}");

            if (isActive)
                ImGui.PopStyleColor();
        }

        ImGui.PopFont();
    }

    private void DrawAllConditions()
    {
        using var font = ImRaii.PushFont(UiBuilder.MonoFont);

        ImGui.TextWrapped("Active conditions will highlight green.");
        ImGui.Separator();

        foreach (ConditionFlag flag in Enum.GetValues(typeof(ConditionFlag)))
        {
            var isActive = Service.Condition[flag];
            if (isActive)
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);

            ImGui.Text($"ID: {(int)flag} Enum: {flag}");

            if (isActive)
                ImGui.PopStyleColor();
        }
    }

    private readonly IEnumerable<ClassJob> classJobSheet = Svc.Data.GetExcelSheet<ClassJob>(Svc.ClientState.ClientLanguage)!.Where(x => !x.Name.RawString.IsNullOrEmpty());
    private void DrawGameData()
    {
        using var font = ImRaii.PushFont(UiBuilder.MonoFont);

        ImGui.TextWrapped("Misc Game Data Information");
        ImGui.Separator();

        ImGui.TextWrapped("ClassJob");
        ImGui.PushStyleColor(ImGuiCol.Text, ShadedColor);
        foreach (var cj in classJobSheet)
        {
            ImGui.Text($"{cj.Name}: Key={cj.RowId}; ExpArrayIndex={cj.ExpArrayIndex}");
        }
        ImGui.PopStyleColor();
    }
}
