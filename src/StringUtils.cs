namespace ExtensionMethods;

public static class StringUtils
{
    public static string Regplace(
        this string str, string pattern, string replacement
    )
    {
        return Regex.Replace(str, pattern, replacement);
    }

    public static bool Match(this string str, string pattern)
    {
        return Regex.IsMatch(str, pattern);
    }

    public static string Join(this string[] strArray, string glue)
    {
        return string.Join(glue, strArray);
    }

    public static int IndexOf(this string[] strArray, string find)
    {
        return Array.IndexOf(strArray, find);
    }
}