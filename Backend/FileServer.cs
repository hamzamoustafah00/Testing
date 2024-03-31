using Microsoft.Extensions.FileProviders;

public class FileServer
{

  public FileServer(WebApplication app, string path)
  {
    app.UseFileServer(new FileServerOptions
    {
      FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", path)
      )
    });
  }

}