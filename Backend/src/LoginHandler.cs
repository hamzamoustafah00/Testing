public static class LoginHandler
{
    public static void Start(WebApplication app)
    {
        app.MapPost("/api/login", (HttpContext context) =>
        {
            var session = new SessionHandler();
            session.SetValue(context, "user", "hepp");
            return Result.encode(session.GetValue(context, "user"));
        });

        app.MapGet("/api/login", (HttpContext context) =>
        {
            var session = new SessionHandler();
            Result.encode(session.GetValue(context, "user"));
        });
    }
}