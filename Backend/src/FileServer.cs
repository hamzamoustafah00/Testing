using Microsoft.Extensions.FileProviders;

public static class FileServer
{
    public static void Start(
        WebApplication app, bool isSpa, params string[] pathParts
    )
    {
        var path = Directory.GetCurrentDirectory();
        foreach (var part in pathParts) { path = Path.Combine(path, part); }

        // Write status codes as response bodies
        // and if the app is an SPA serve index.html on non-file 404:s
        app.UseStatusCodePages(async statusCodeContext =>
        {
            var context = statusCodeContext.HttpContext;
            var request = context.Request;
            var response = context.Response;
            var statusCode = response.StatusCode;
            var isInApi = request.Path.StartsWithSegments("/api");
            var isFilePath = (request.Path + "").Contains(".");
            var type = isInApi || statusCode != 404 ?
                "application/json; charset=utf-8" : "text/html";
            var error = statusCode == 404 ?
                "404. Not found." : "Status code: " + statusCode;

            response.ContentType = type;
            if (isSpa && !isInApi && !isFilePath && statusCode == 404)
            {
                response.StatusCode = 200;
                await response.WriteAsync(
                    File.ReadAllText(Path.Combine(path, "index.html"))
                );
            }
            else
            {
                await response.WriteAsJsonAsync(new { error });
            }
        });

        // Serve static frontend files
        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider(path)
        });

        // Get a list of files from a subfolder in the frontend
        app.MapGet("/api/files/{folder}", (HttpContext context, string folder) =>
        {
            object result;
            try
            {
                result = Directory.GetFiles(
                    Path.Combine(path, folder))
                    .Select(x => x.Split('/').ToList().Last())
                    .Where(x => CheckAcl.Allow(context, "GET", "/content/" + x));
            }
            catch (Exception) { result = new { error = "No such content." }; }
            return result;
        });
    }
}