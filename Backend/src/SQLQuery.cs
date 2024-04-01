using Microsoft.Data.Sqlite;

public static class SQLQuery
{
    private static SqliteConnection db =
        new SqliteConnection("Data Source=_db.sqlite3");

    public static List<DynObject> All(
        string sql, params object[] parameters
    )
    {
        db.Open();

        // Prepare statement/command
        var command = db.CreateCommand();
        command.CommandText = @sql;
        for (var i = 0; i < parameters.Length; i += 2)
        {
            command.Parameters.AddWithValue(
                (string)parameters[i], parameters[i + 1]
            );
        }

        // Run query and collect result
        var rows = new List<DynObject>();
        try
        {
            // Non-SELECT queries
            if (sql.Split(" ")[0].ToUpper() != "SELECT")
            {
                rows.Add(new DynObject(new
                {
                    command = sql.Split(" ")[0].ToUpper(),
                    rowsAffected = command.ExecuteNonQuery()
                }));
            }
            // SELECT queries
            else
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new DynObject(reader));
                    }
                }
            }
        }
        // Handle exceptions by creating an error
        // (also see Result.cs for how status code are set)
        catch (Exception error)
        {
            rows = new List<DynObject>() {
                new DynObject(new { error = error.Message.Split("'")[1] })
            };
        }
        return rows;
    }

    public static DynObject Run(
        string sql, params object[] parameters
    )
    {
        // Return the first object in the list 
        // or null if the list is empty
        var result = All(sql, parameters);
        DynObject resultOne;
        try { resultOne = ((List<DynObject>)result)[0]; }
        catch (Exception) { return default!; /*default! == null*/}
        return resultOne;
    }
}