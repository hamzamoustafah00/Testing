public class LoginHandler
{

  public LoginHandler(WebApplication app)
  {
    app.MapPost("/api/login", () =>
    {
      SessionHandler.SetValue("user", "hepp");
      return Result.encode(SessionHandler.GetValue("user"));
    });

    app.MapGet("/api/login", () =>
      Result.encode(SessionHandler.GetValue("user"))
    );
  }

}