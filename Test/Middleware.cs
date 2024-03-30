public class Middleware
{
  public static string cookieValue = "";

  public Middleware(WebApplication app, string serverName)
  {
    app.Use(async (context, next) =>
    {
      context.Response.OnStarting(() =>
      {
        SetServerHeader(context, serverName);
        EnsureCookie(context);
        return Task.CompletedTask;
      });

      await next();
    });
  }

  public void SetServerHeader(HttpContext context, string serverName)
  {
    var res = context.Response;
    res.Headers.Append("Server", serverName);
  }

  public void EnsureCookie(HttpContext context)
  {
    var request = context.Request;
    var response = context.Response;

    string? temp;
    request.Cookies.TryGetValue("session", out temp);
    cookieValue = temp == null ? "" : temp;

    if (cookieValue == "")
    {
      cookieValue = Guid.NewGuid().ToString();
      response.Cookies.Append("session", cookieValue);
    }

  }

  public static string GetCookieValue()
  {
    return cookieValue;
  }

}