namespace Backend;

public static class LoginRoutes
{
    private static DynObject? GetUser(HttpContext context)
    {
        var user = Session.Get(context, "user");
        return user.GetStr("id") != "[undefined]" ? user : null;
    }

    public static void Start(WebApplication app)
    {
        app.MapPost("/api/login", (HttpContext context, JsonElement bodyJson) =>
        {
            var user = GetUser(context);
            var body = new DynObject(bodyJson.ToString());

            // If there is a user logged in already
            if (user != null)
            {
                var already = new { error = "A user is already logged in." };
                return Result.encode(already);
            }

            // Find the user in the DB
            var dbUser = SQLQuery.Run(
                "SELECT * FROM users WHERE email = $email",
                "email", body.GetStr("email")
            );
            if (dbUser == null)
            {
                return Result.encode(new { error = "No such user." });
            }

            // If the password doesn't match
            if (!Password.Verify(
                (string)body.Get("password"),
                (string)dbUser.Get("password")
            ))
            {
                return Result.encode(new { error = "Password mismatch." });
            }

            // Add the user to the session, without password
            dbUser.Delete("password");
            Session.Set(context, "user", dbUser!);

            // Return the user
            return Result.encode(dbUser!);
        });

        app.MapGet("/api/login", (HttpContext context) =>
        {
            var user = GetUser(context);
            return Result.encode(user == null ?
                new { error = "No user is logged in." } :
                user
            );
        });

        app.MapDelete("/api/login", (HttpContext context) =>
        {
            var user = GetUser(context);

            // Delete the user from the session
            if (user != null)
            {
                Session.Set(context, "user", null!);
            }

            return Result.encode(user == null ?
                new { error = "No user is logged in." } :
                new { status = "Successful logout." }
            );
        });
    }
}