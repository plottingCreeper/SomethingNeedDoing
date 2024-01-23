using SomethingNeedDoing.Exceptions;
using SomethingNeedDoing.Grammar.Modifiers;
using SomethingNeedDoing.Misc;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SomethingNeedDoing.Grammar.Commands;

/// <summary>
/// The /target command.
/// </summary>
internal class TargetCommand : MacroCommand
{
    private static readonly Regex Regex = new(@"^/target\s+(?<name>.*?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string targetName;
    private readonly int targetIndex;
    private readonly int listIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetCommand"/> class.
    /// </summary>
    /// <param name="text">Original text.</param>
    /// <param name="targetName">Target name.</param>
    /// <param name="wait">Wait value.</param>
    /// <param name="index">Object index.</param>
    private TargetCommand(string text, string targetName, WaitModifier wait, IndexModifier index, ListIndexModifier listIndex)
        : base(text, wait, index)
    {
        this.targetIndex = index.ObjectId;
        this.targetName = targetName.ToLowerInvariant();
        this.listIndex = listIndex.ListIndex;
    }

    /// <summary>
    /// Parse the text as a command.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <returns>A parsed command.</returns>
    public static TargetCommand Parse(string text)
    {
        _ = WaitModifier.TryParse(ref text, out var waitModifier);
        _ = IndexModifier.TryParse(ref text, out var indexModifier);
        _ = ListIndexModifier.TryParse(ref text, out var listIndexModifier);
        var match = Regex.Match(text);
        if (!match.Success)
            throw new MacroSyntaxError(text);

        var nameValue = ExtractAndUnquote(match, "name");
        return new TargetCommand(text, nameValue, waitModifier, indexModifier, listIndexModifier);
    }

    /// <inheritdoc/>
    public override async Task Execute(ActiveMacro macro, CancellationToken token)
    {
        Service.Log.Debug($"Executing: {this.targetIndex}");
        var target = Service.ObjectTable
            .OrderBy(o => Vector3.Distance(o.Position, Service.ClientState.LocalPlayer!.Position))
            .Where(obj => obj.Name.TextValue.ToLowerInvariant() == this.targetName && obj.IsTargetable && (this.targetIndex <= 0 || obj.ObjectIndex == this.targetIndex))
            .Skip(this.listIndex)
            .FirstOrDefault();


        if (target == default && Service.Configuration.StopMacroIfTargetNotFound)
            throw new MacroCommandError("Could not find target");

        if (target != default)
            Service.TargetManager.Target = target;

        await this.PerformWait(token);
    }
}
