namespace Backend;

public static class ErrorHandler
{
    public static void Start(WebApplication app, string serverName)
    {
        app.UseExceptionHandler((exceptionApp) =>
        {
            exceptionApp.Run(async context =>
            {
                var feature = context.Features
                    .Get<IExceptionHandlerPathFeature>();
                context.Response.Headers.Append("Server", serverName);
                await context.Response.WriteAsJsonAsync(
                    new { error = feature!.Error.Message }
                );
            });
        });
    }
}