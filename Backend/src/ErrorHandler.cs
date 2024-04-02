using Microsoft.AspNetCore.Diagnostics;

public static class ErrorHandler
{
    public static void Start(WebApplication app)
    {
        app.UseExceptionHandler((exceptionApp) =>
        {
            exceptionApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                await context.Response.WriteAsJsonAsync(
                    new { error = feature!.Error.Message }
                );
            });
        });
    }
}