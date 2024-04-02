
using System.Text.Json;

public static class REST
{
    private static DynObject ReqBodyParse(string table, DynObject body)
    {
        // Filter body: Always remove "id" + remove "role" in users table
        var keys = body.GetKeys().Where(key =>
            key != "id" && (table != "users" || key != "role")
        );
        // Return dynamically created column specific parts of SQL queries
        return new DynObject(new
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
            var body = new DynObject(bodyJson.ToString());
            var parsed = ReqBodyParse(table, body);
            var columns = parsed.GetStr("insertColumns");
            var values = parsed.GetStr("insertValues");
            var sql = $"INSERT INTO {table}({columns}) VALUES({values})";
            var result = SQLQuery.Run(sql, body.ToQueryParams());
            if (!result.HasKey("error"))
            {
                result.Set("insertId", SQLQuery.Run(
                    @$"SELECT id AS __insertId 
                       FROM {table} ORDER BY id DESC LIMIT 1"
                ).GetInt("__insertId"));
            }
            return Result.encode(result);
        });

        app.MapGet("/api/{table}", (HttpContext context, string table) =>
        {
            return Result.encode(SQLQuery.All($"SELECT * FROM {table}"));
        });

        app.MapGet("/api/{table}/{id}", (string table, string id) =>
            Result.encode(SQLQuery.Run(
                $"SELECT * FROM {table} WHERE id = $id",
                "id", id
            ))
        );

        app.MapPut("/api/{table}/{id}", (
            string table, string id, JsonElement bodyJson
        ) =>
        {
            var body = new DynObject(bodyJson.ToString());
            var parsed = ReqBodyParse(table, body);
            var update = parsed.GetStr("update");
            var sql = $"UPDATE {table} SET {update} WHERE id = $id";
            var result = SQLQuery.Run(sql, body.ToQueryParams());
            return Result.encode(result);
        });

        app.MapDelete("/api/{table}/{id}", (string table, int id) =>
            Result.encode(SQLQuery.Run(
                $"DELETE FROM {table} WHERE id = $id",
                "id", id
            ))
        );
    }
}