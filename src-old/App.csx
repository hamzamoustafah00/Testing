var serverName = "ironboy's minimal API server";
var isSpa = true;
Debug.on = true;
CheckAcl.on = true;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();


// Middleware: Set the server name response header 
// and touch the user session with a new timestamp
// and apply ACL rules to check if access is allowd
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Server", serverName);
    Debug.Log("route", context);
    Session.Touch(context);
    if (CheckAcl.Allow(context)) { await next(context); }
    else
    {
        // ACL says the route is not allowed
        context.Response.StatusCode = 405;
        await context.Response.WriteAsJsonAsync(
            new { error = "Not allowed." }
        );
    }
});

// Set up routes, error handling, ACL and session purging
ErrorHandler.Start(app, serverName);
FileServer.Start(app, isSpa, "..", "Frontend");
LoginRoutes.Start(app);
RestNew.Start(app);
CheckAcl.Start();
//Session.DeleteOldSessions(2);

// Start the server on port 3001
app.Run("http://localhost:3001");