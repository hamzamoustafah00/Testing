using Microsoft.Extensions.FileProviders;

public static class FileServer
{
    public static void Start(WebApplication app, params string[] pathParts)
    {
        var path = Directory.GetCurrentDirectory();
        foreach (var part in pathParts) { path = Path.Combine(path, part); }
        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider(path)
        });
    }
}