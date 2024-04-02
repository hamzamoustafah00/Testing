var serverName = "ironboy's minimal API server";
var builder = WebApplication.CreateBuilder(args);

// Always throw custom errors, in dev and production
builder.Services.Configure<RouteHandlerOptions>
    (o => o.ThrowOnBadRequest = true);

var app = builder.Build();

// Middleware to set server name response header 
// and touch the user session with new timestamp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Server", serverName);
    Session.Touch(context);
    await next(context);
});

// Set up routes, error handling and session purging
ErrorHandler.Start(app);
FileServer.Start(app, "..", "Frontend");
LoginRoutes.Start(app);
REST.Start(app);
Session.DeleteOldSessions(2);

// Start the server on port 3001
app.Run("http://localhost:3001");