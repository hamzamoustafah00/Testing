using System.Diagnostics;

namespace WebApp;
public static class Server
{
    public static void Start()
    {
        var builder = WebApplication.CreateBuilder();
        App = builder.Build();
        Middleware();
        DebugLog.Start();
        Acl.Start();
        ErrorHandler.Start();
        FileServer.Start();
        LoginRoutes.Start();
        RestApi.Start();
        Session.Start();
        // Start the server on port 3001
        var runUrl = "http://localhost:" + Globals.port;
        Log("Server running on:", runUrl);
        Log("With these settings:", Globals);
        App.Run(runUrl);
    }

    // A basic middleware that changes the server response header,
    // initiates the debug logging for the request,
    // keep sessions alive and stop a route if acl does not approve of it,
    // and adds info for debugging
    public static void Middleware()
    {
        App.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Server", (string)Globals.serverName);
            DebugLog.Register(context);
            Session.Touch(context);
            if (!Acl.Allow(context))
            {
                // Acl says the route is not allowed
                context.Response.StatusCode = 405;
                var error = "Not allowed.";
                await context.Response.WriteAsJsonAsync(new { error });
            }
            else { await next(context); }
            var res = context.Response;
            // Add info for debugging
            string body = string.Empty;
            DebugLog.Add(context, new
            {
                statusCode = res.StatusCode,
                contentType = res.ContentType,
                contentLength = res.ContentLength,
            });
        });
    }
}