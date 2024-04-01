public static class SessionHandle
{
    private static string GetCookieValue(HttpContext context)
    {
        // Note: context.Items is request specific!
        var value = context.Items["cookieValue"] as string;
        if (value == null)
        {
            // This is the first call during this request to
            // GetCookieValue, so get the cookieValue from the request!
            context.Request.Cookies.TryGetValue("session", out value);
        }
        if (value == null)
        {
            // There is no cookie set yet for this client. Set one!
            value = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("session", value);
        }
        // Check if the DB has a session with this cookie as id
        var exists = SQLQuery.Run(
            "SELECT * FROM sessions WHERE id = $id",
            "id", value
        );
        if (exists == null)
        {
            // Otherwise store a new session in the DB
            SQLQuery.Run(
                 "INSERT INTO sessions(id) VALUES($id)",
                 "id", value
            );
        }
        context.Items["cookieValue"] = value;
        return value;
    }

    public static void Touch(HttpContext context)
    {
        var cookieValue = GetCookieValue(context);
        // Touch the session - set modified to now!
        SQLQuery.Run(
            @"UPDATE sessions SET modified = DATETIME('now')
              WHERE id = $id",
            "id", cookieValue
        );
    }
}