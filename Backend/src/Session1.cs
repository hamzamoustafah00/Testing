
public static partial class Session
{
    private static DynObject GetRawSession(HttpContext context)
    {
        // If the session is already cached in context.Items
        // (if we call this method more than once per request)
        var inContext = context.Items["session"];
        if (inContext is DynObject)
        {
            return (DynObject)inContext;
        }

        // Get the cookie value if we have a session cookie
        // - otherwise create a session cookie
        string? cookieValue;
        context.Request.Cookies.TryGetValue("session", out cookieValue);
        if (cookieValue == null)
        {
            cookieValue = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("session", cookieValue);
        }

        // Get the session data from the database if it stored there
        // otherwise store it in the database
        var session = SQLQuery.Run(
          "SELECT * FROM sessions WHERE id = $id",
          "id", cookieValue
        );
        if (session == null)
        {
            SQLQuery.Run(
               "INSERT INTO sessions(id) VALUES($id)",
               "id", cookieValue
            );
            session = new DynObject();
        }

        // Cache the session in context.Items
        context.Items["session"] = session;

        return session;
    }

    public static DynObject Get(HttpContext context, string key)
    {
        var session = GetRawSession(context);
        // Convert the data from JSON to DynObject
        var data = new DynObject(session.Get("data"));
        // Return the requested data key/property as a DynObject
        return new DynObject(data.Get(key));
    }

    public static void Set(HttpContext context, string key, object value)
    {
        var session = GetRawSession(context);
        // Convert the data from JSON to DynObject
        var data = new DynObject(session.Get("data"));
        // Set the property in data
        data.Set(key, value);
        // Save to DB, with the data converted to JSON
        SQLQuery.Run(
           @"UPDATE sessions 
             SET modified = DATETIME('now'), data = $data
             WHERE id = $id",
           "id", GetRawSession(context).Get("id"),
           "data", data.ToJson()
       );
    }
}