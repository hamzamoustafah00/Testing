namespace WebApp;
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

    public static int ToInt(this string str)
    {
        return Int32.Parse(str);
    }
}