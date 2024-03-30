using Microsoft.AspNetCore.Diagnostics;

public class ErrorHandler
{

  public ErrorHandler(WebApplication app)
  {
    app.UseExceptionHandler((exceptionApp) =>
    {
      exceptionApp.Run(async context =>
      {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (feature != null)
        {
          await context.Response.WriteAsJsonAsync(
            new { error = feature.Error.Message }
          );
        }
        else
        {
          await context.Response.WriteAsJsonAsync(
            new { error = "Something went wrong." }
         );
        }
      });
    });
  }

}