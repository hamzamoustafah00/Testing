// Global settings
Globals = Obj(new
{
    debugOn = true,
    aclOn = true,
    isSpa = true,
    port = 3001,
    frontendPath = Path.Combine("..", "Frontend")
});
Log("Settings:", Globals);

Server.Start();