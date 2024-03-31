/*var builder = WebApplication.CreateBuilder(args);

// Always throw custom errors (in dev and production)
builder.Services.Configure<RouteHandlerOptions>
 (o => o.ThrowOnBadRequest = true);

var app = builder.Build();

new Middleware(app, "ironboy's minimal API server");
new ErrorHandler(app);
new FileServer(app, "Frontend");
new LoginHandler(app);
new REST(app);
SessionHandler.DeleteOldSessions(2);

app.Run("http://localhost:3001");*/

var a = new DynObject(new
{
  name = "Anna",
  age = 42,
  friends = new List<string>() { "Eva", "Erik" },
  hobby = new
  {
    name = "Fishing",
    cool = true
  }
});

// Console.WriteLine(a.ToJson());
Console.WriteLine(JSON.Stringify(a));
Console.WriteLine(a.Get("hobby.name"));
Console.WriteLine(a.Get("friends.1"));