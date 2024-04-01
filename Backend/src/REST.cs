
using System.Text.Json;

public class REST
{

  private DynObject ReqBodyParse(string table, DynObject body)
  {

    var keys = body.GetKeys().Where(
      key => key != "id" && (table != "users" || key != "role")
    );
    return new DynObject(new
    {
      insertColumns = string.Join(",", keys),
      insertValues = "$" + string.Join(",$", keys),
      update = string.Join(",", keys.Select(key => $"${key}={key}"))
    });
  }

  public REST(WebApplication app)
  {

    app.MapPost("/api/{table}", (string table, JsonElement bodyJson) =>
    {
      var body = new DynObject(bodyJson.ToString());
      var parsed = ReqBodyParse(table, body);
      var columns = parsed.GetStr("insertColumns");
      var values = parsed.GetStr("insertValues");
      var sql = $"INSERT INTO {table}({columns}) VALUES({values})";
      var result = SQLQuery.RunOne(sql, body.ToQueryParams());
      if (!result.HasKey("error"))
      {
        result.Set("insertId", SQLQuery.RunOne(
          $"SELECT id FROM {table} ORDER BY id DESC LIMIT 1"
        ).GetInt("id"));
      }
      return Result.encode(result);
    });

    app.MapGet("/api/{table}", (string table) =>
      Result.encode(SQLQuery.Run(
        $"SELECT * FROM {table}"
      ))
    );

    app.MapGet("/api/{table}/{id}", (string table, int id) =>
      Result.encode(SQLQuery.RunOne(
        $"SELECT * FROM {table} WHERE id = $id",
        "id", id
      ))
    );

    app.MapPut("/api/{table}/{id}", (
      string table, int id, JsonElement bodyJson
    ) =>
    {
      var body = new DynObject(bodyJson.ToString());
      var parsed = ReqBodyParse(table, body);
      var update = parsed.GetStr("update");
      var sql = $"UPDATE {table} SET {update} WHERE id = $id";
      var result = SQLQuery.RunOne(sql, body.ToQueryParams());
      return Result.encode(result);
    });

    app.MapDelete("/api/{table}/{id}", (string table, int id) =>
      Result.encode(SQLQuery.RunOne(
        $"DELETE FROM {table} WHERE id = $id",
        "id", id
      ))
    );
  }

}