using System.Globalization;
using System.Text.RegularExpressions;

namespace SomethingNeedDoing.Grammar.Modifiers;

/// <summary>
/// The &lt;list index&gt; modifier.
/// </summary>
internal class ListIndexModifier : MacroModifier
{
    private static readonly Regex Regex = new(@"(?<modifier><list\.(?<listIndex>\d+(?:\.\d+)?)>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private ListIndexModifier(int listIndex)
    {
        this.ListIndex = listIndex;
    }

    public int ListIndex { get; }

    /// <summary>
    /// Parse the text as a modifier.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <param name="command">A parsed modifier.</param>
    /// <returns>A value indicating whether the modifier matched.</returns>
    public static bool TryParse(ref string text, out ListIndexModifier command)
    {
        var match = Regex.Match(text);
        var success = match.Success;

        if (!success)
        {
            command = new ListIndexModifier(0);
            return false;
        }

        var group = match.Groups["modifier"];
        text = text.Remove(group.Index, group.Length);

        var indexGroup = match.Groups["listIndex"];
        var indexValue = indexGroup.Value;
        var index = int.Parse(indexValue, CultureInfo.InvariantCulture);

        command = new ListIndexModifier(index);
        return true;
    }
}
