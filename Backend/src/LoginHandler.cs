public static class LoginHandler
{
    public static void Start(WebApplication app)
    {
        app.MapPost("/api/login", (HttpContext context) =>
        {
            Session.Set(context, "user", new { firstName = "Joe", lastName = "Doe" });
            return Result.encode(new { ok = true });
        });

        app.MapGet("/api/login", (HttpContext context) =>
        {
            return Result.encode(Session.Get(context, "user"));
        });
    }
}