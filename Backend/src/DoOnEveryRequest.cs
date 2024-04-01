public class DoOnEveryRequest
{
    public DoOnEveryRequest(WebApplication app, string serverName)
    {
        // Middleware that affects all requests
        app.Use(async (context, next) =>
        {
            SetServerHeader(context, serverName);
            //TouchSession(context);
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