namespace Backend;

public static class RestNew
{
    private static dynamic ReqBodyParse(string table, Obj body)
    {
        // Filter body: Always remove "id" + remove "role" in users table
        var keys = body.GetKeys().Filter(key =>
            key != "id" && (table != "users" || key != "role")
        );
        // Return dynamically created column specific parts of SQL queries
        return Obj(new
        {
            insertColumns = string.Join(",", keys),
            insertValues = "$" + string.Join(",$", keys),
            update = string.Join(",", keys.Select(key => $"${key}={key}"))
        });
    }

    public static void Start(WebApplication app)
    {
        app.MapPost("/api/{table}", (string table, JsonElement bodyJson) =>
        {
            var body = JSON.Parse(bodyJson.ToString());
            var parsed = ReqBodyParse(table, body);
            var columns = parsed.insertColumns;
            var values = parsed.insertValues;
            var sql = $"INSERT INTO {table}({columns}) VALUES({values})";
            var result = SQLQueryOne(sql, body);
            if (!result.HasKey("error"))
            {
                // Get the insert id and add to our result
                result.insertId = SQLQueryOne(
                    @$"SELECT id AS __insertId 
                       FROM {table} ORDER BY id DESC LIMIT 1"
                ).GetInt("__insertId");
            }
            return Result.encode(result);
        });

        app.MapGet("/api/{table}", (HttpContext context, string table) =>
        {
            var q = context.Request.Query;
            var sql = $"SELECT * FROM {table}";
            var extras = new RestExtras(
                q["where"]!, q["orderby"]!, q["limit"]!, q["offset"]!
            );
            sql += extras.sql;
            return Result.encode(SQLQuery(sql, extras.paramObj));
        });

        app.MapGet("/api/{table}/{id}", (string table, string id) =>
            Result.encode(SQLQueryOne(
                $"SELECT * FROM {table} WHERE id = $id", new { id }
            ))
        );

        app.MapPut("/api/{table}/{id}", (
            string table, string id, JsonElement bodyJson
        ) =>
        {
            var body = JSON.Parse(bodyJson.ToString());
            var parsed = ReqBodyParse(table, body);
            var update = parsed.update;
            var sql = $"UPDATE {table} SET {update} WHERE id = $id";
            var result = SQLQueryOne(sql, body.ToQueryParams());
            return Result.encode(result);
        });

        app.MapDelete("/api/{table}/{id}", (string table, string id) =>
            Result.encode(SQLQueryOne(
                $"DELETE FROM {table} WHERE id = $id", new { id }
            ))
        );
    }
}