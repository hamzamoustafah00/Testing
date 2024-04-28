namespace WebApp;
public static partial class Shared
{
    // A global object to store settings in
    private static Obj _globals = Obj();

    public static dynamic Globals
    {
        get { return _globals; }
        set { _globals = value; }
    }

    // A setter/getter for the WebApp
    // (created by Server.cs)
    private static WebApplication _app;

    public static WebApplication App
    {
        get { return _app!; }
        set { _app = value; }
    }

    // Methods for testing if a string
    // contains an integer or a double/decimal number
    [GeneratedRegex(@"^\d{1,}$")]
    private static partial Regex MatchInt();

    [GeneratedRegex(@"^[\d\.]{1,}$")]
    private static partial Regex MatchDouble();

    public static bool IsInt(string x)
    {
        return MatchInt().IsMatch(x);
    }

    public static bool IsDouble(string x)
    {
        return MatchDouble().IsMatch(x);
    }
}