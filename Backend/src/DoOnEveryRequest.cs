public static class DoOnEveryRequest
{
    public static void Start(WebApplication app, string serverName)
    {
        // Middleware that affects all requests
        app.Use(async (context, next) =>
        {
            SetServerHeader(context, serverName);
            SessionHandle.Touch(context);
            await next(context);
        });
    }

    public static void SetServerHeader(HttpContext context, string serverName)
    {
        var res = context.Response;
        res.Headers.Append("Server", serverName);
    }

    public static void TouchSession(HttpContext context)
    {
        var session = new SessionHandler();
        session.Touch(context);
    }
}