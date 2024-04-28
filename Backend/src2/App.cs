// Global settings
Globals = Obj(new
{
    debugOn = true,
    aclOn = true,
    isSpa = true,
    port = 3001,
    serverName = "ironboy's minimal API server",
    frontendPath = Path.Combine("..", "Frontend"),
    sessionLifeTimeHours = 2
});

Server.Start();