using System.Collections.Generic;

namespace SomethingNeedDoing.Misc;

/// <summary>
/// Extension methods.
/// </summary>
internal static class Extensions
{
    /// <inheritdoc cref="string.Join(char, string?[])"/>
    public static string Join(this IEnumerable<string> values, char separator)
        => string.Join(separator, values);

    /// <inheritdoc cref="string.Join(string, string?[])"/>
    public static string Join(this IEnumerable<string> values, string separator)
        => string.Join(separator, values);

    public static string GetLast(this string source, int tail_length)
    {
        if (tail_length >= source.Length)
            return source;
        return source.Substring(source.Length - tail_length);
    }
}
