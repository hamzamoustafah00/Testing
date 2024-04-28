namespace WebApp;
public static class Server
{
    public static void Start()
    {
        var builder = WebApplication.CreateBuilder();
        App = builder.Build();
        // Set up routes, error handling, ACL and session purging
        /*ErrorHandler.Start(app, serverName);
        FileServer.Start(app, isSpa, "..", "Frontend");
        LoginRoutes.Start(app);
        RestNew.Start(app);
        CheckAcl.Start();*/
        //Session.DeleteOldSessions(2);

        FileServer.Start();
        RestApi.Start();

        // Start the server on port 3001
        App.Run("http://localhost:" + Globals.port);
    }
}