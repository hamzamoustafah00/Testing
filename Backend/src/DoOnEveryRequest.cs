public static class DoOnEveryRequest
{
    public static void Start(WebApplication app, string serverName)
    {
        // Middleware that affects all requests
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Server", serverName);
            Session.Touch(context);
            await next(context);
        });
    }
}