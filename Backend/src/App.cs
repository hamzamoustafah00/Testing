// Global settings
Globals = Obj(new
{
    debugOn = true,
    detailedAclDebug = false,
    aclOn = false,
    isSpa = true,
    port = 3001,
    serverName = "Ironboy's Minimal API Server",
    frontendPath = FilePath("..", "Frontend"),
    sessionLifeTimeHours = 2
});

// Example of how you can run tests as if they were part of the main program
// This solves the problem with console output not working when running tests
// using "dotnet test". But in the end we want to run the tests by calling
// "dotnet test" - so it's only useful during the development of the tests!
// Remove the comments for this line and comment out "Server.Start()":
// new UtilsTest().TestCreateMockUsers();

Server.Start();