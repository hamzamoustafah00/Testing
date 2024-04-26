namespace Backend;

public static partial class SQLQueryNew
{

    [GeneratedRegex(@"^\d{1,}$")]
    private static partial Regex MatchInt();

    [GeneratedRegex(@"^[\d\.]{1,}$")]
    private static partial Regex MatchDouble();

    private static SqliteConnection db =
        new SqliteConnection("Data Source=_db.sqlite3");

    static SQLQueryNew() { db.Open(); }

    private static dynamic ObjFromReader(SqliteDataReader reader)
    {
        var obj = Obj();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var key = reader.GetName(i);
            var str = reader.GetString(i);
            object value =
                MatchInt().IsMatch(str) ? reader.GetInt64(i) :
                MatchDouble().IsMatch(str) ? reader.GetDouble(i) :
                str;
            obj[key] = value;
        }
        return obj;
    }

    public static Arr SQLQuery(string sql, object parameters = null!)
    {
        var paras = parameters == null ? Obj() : Obj(parameters);
        var command = db.CreateCommand();
        command.CommandText = @sql;
        var entries = (Arr)paras.GetEntries();
        entries.ForEach(x => command.Parameters.AddWithValue(x[0], x[1]));
        Log(new { SQL = sql, parameters = paras });
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

    public static dynamic SQLQueryOne(string sql, object parameters = null!)
    {
        return SQLQuery(sql, parameters)[0];
    }
}