namespace Backend;
public static partial class Session
{
    private static bool deleteLoopStarted = false;

    public static async void DeleteOldSessions(int timeToLiveHours)
    {
        // only start the loop once...
        if (deleteLoopStarted) { return; }
        deleteLoopStarted = true;

        while (true)
        {
            SQLQuery.Run(
                @$"DELETE FROM sessions WHERE 
                   DATETIME('now', '-{timeToLiveHours} hours') > modified"
            );
            // Wait one minute per next check
            await Task.Delay(60000);
        }
    }

    // Touch the session - set modified to now!
    public static void Touch(HttpContext context)
    {
        SQLQuery.Run(
            @"UPDATE sessions SET modified = DATETIME('now')
              WHERE id = $id",
            "id", GetRawSession(context).Get("id")
        );
    }
}