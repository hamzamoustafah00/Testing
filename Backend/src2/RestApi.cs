namespace WebApp;
public static class RestApi
{
    public static void Start()
    {
        App.MapPost("/api/{table}", (string table, JsonElement bodyJson) =>
        {
            var body = JSON.Parse(bodyJson.ToString());
            body.delete("id");
            var parsed = ReqBodyParse(table, body);
            var columns = parsed.insertColumns;
            var values = parsed.insertValues;
            var sql = $"INSERT INTO {table}({columns}) VALUES({values})";
            var result = SQLQueryOne(sql, parsed.body);
            if (!result.HasKey("error"))
            {
                // Get the insert id and add to our result
                result.insertId = SQLQueryOne(
                    @$"SELECT id AS __insertId 
                       FROM {table} ORDER BY id DESC LIMIT 1"
                ).GetInt("__insertId");
            }
            return RestResult.Parse(result);
        });

        App.MapGet("/api/{table}", (HttpContext context, string table) =>
        {
            var sql = $"SELECT * FROM {table}";
            var query = RestQuery.Parse(context.Request.Query);
            sql += query.sql;
            return RestResult.Parse(SQLQuery(sql, query.parameters));
        });

        App.MapGet("/api/{table}/{id}", (string table, string id) =>
            RestResult.Parse(SQLQueryOne(
                $"SELECT * FROM {table} WHERE id = $id",
                ReqBodyParse(table, Obj(new { id })).body
            ))
        );

        App.MapPut("/api/{table}/{id}", (
            string table, string id, JsonElement bodyJson
        ) =>
        {
            var body = JSON.Parse(bodyJson.ToString());
            body.id = id;
            var parsed = ReqBodyParse(table, body);
            var update = parsed.update;
            var sql = $"UPDATE {table} SET {update} WHERE id = $id";
            var result = SQLQueryOne(sql, parsed.body);
            return RestResult.Parse(result);
        });

        App.MapDelete("/api/{table}/{id}", (string table, string id) =>
            RestResult.Parse(SQLQueryOne(
                $"DELETE FROM {table} WHERE id = $id",
                ReqBodyParse(table, Obj(new { id })).body
            ))
        );
    }
}