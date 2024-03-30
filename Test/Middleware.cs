public class Middleware
{

  public string serverName = "ironboy's minimal API server";

  public Middleware(WebApplication app)
  {
    app.Use(async (context, next) =>
    {
      context.Response.OnStarting(() =>
      {
        SetServerHeader(context);
        return Task.CompletedTask;
      });

      await next();
    });
  }

  public void SetServerHeader(HttpContext context)
  {
    context.Response.Headers.Append("Server", serverName);
    context.Response.Cookies.Append("myCookie", "123");
  }

}