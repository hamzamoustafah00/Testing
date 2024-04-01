
public static partial class Session
{
    // Touch the session - set modified to now!
    public static void Touch(HttpContext context)
    {
        SQLQuery.Run(
            @"UPDATE sessions SET modified = DATETIME('now')
              WHERE id = $id",
            "id", GetRawSession(context).Get("id")
        );
    }

    // Only call once for the whole app
    public static async void DeleteOldSessions(int timeToLiveHours)
    {
        while (true)
        {
            SQLQuery.Run(
                @$"DELETE FROM sessions WHERE 
                   DATETIME('now', '-{timeToLiveHours} hours') > modified"
            );
            // Check once a minute
            await Task.Delay(60000);
        }
    }
}