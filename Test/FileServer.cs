using Microsoft.Extensions.FileProviders;

public class FileServer
{

  public FileServer(WebApplication app, string folder)
  {
    app.UseFileServer(new FileServerOptions
    {
      FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), folder)
      )
    });
  }

}