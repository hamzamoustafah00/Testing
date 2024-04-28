namespace WebApp;
public static class DbQuery
{
    // Setup the database connection
    private static SqliteConnection db =
        new SqliteConnection("Data Source=_db.sqlite3");

    static DbQuery() { db.Open(); }

    // Helper to create an object from the DataReader
    private static dynamic ObjFromReader(SqliteDataReader reader)
    {
        var obj = Obj();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var key = reader.GetName(i);
            obj[key] = reader.GetString(i).TryToNum();
        }
        return obj;
    }

    // Run a query - rows are returned as an arry of objects
    public static Arr SQLQuery(string sql, object parameters = null)
    {
        var paras = parameters == null ? Obj() : Obj(parameters);
        var command = db.CreateCommand();
        command.CommandText = @sql;
        var entries = (Arr)paras.GetEntries();
        entries.ForEach(x => command.Parameters.AddWithValue(x[0], x[1]));
        // Log(new { SQL = sql.Regplace(@"\s+", " "), parameters = paras });
        var rows = Arr();
        try
        {
            if (sql.StartsWith("SELECT ", true, null))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rows.Push(ObjFromReader(reader));
                }
            }
            else
            {
                rows.Push(new
                {
                    command = sql.Split(" ")[0].ToUpper(),
                    rowsAffected = command.ExecuteNonQuery()
                });
            }
        }
        catch (Exception err)
        {
            rows.Push(new { error = err.Message.Split("'")[1] });
        }
        return rows;
    }

    // Run a query - only return the first row, as an object
    public static dynamic SQLQueryOne(string sql, object parameters = null)
    {
        return SQLQuery(sql, parameters)[0];
    }
}