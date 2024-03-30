var builder = WebApplication.CreateBuilder(args);

// Always throw custom errors (in dev and production)
builder.Services.Configure<RouteHandlerOptions>
 (o => o.ThrowOnBadRequest = true);

var app = builder.Build();

new ErrorHandler(app);
new FileServer(app, "www");
new REST(app);

app.Run("http://localhost:3001");