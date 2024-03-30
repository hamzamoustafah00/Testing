
using System.Text.Json;

public class REST
{

  public REST(WebApplication app)
  {

    app.MapPost("/api/{table}", (string table, JsonElement body) =>
    {
      var parameters = Utils.JSONToArray(body.ToString());
      var columns = "";
      var values = "";
      for (var i = 0; i < parameters.Length; i += 2)
      {
        var column = (string)parameters[i];
        if (column.ToLower() == "id") { continue; }
        columns += (i != 0 ? ", " : "") + column;
        values += (i != 0 ? ", " : "") + "$" + column;
      }
      var sql = $"INSERT INTO {table}({columns}) VALUES({values})";
      var result = SQLQuery.RunOne(sql, parameters);
      if (!Utils.HasProperty(result, "error"))
      {
        var insertId = Utils.GetPropertyValue(SQLQuery.RunOne(
          $"SELECT id FROM {table} ORDER BY id DESC LIMIT 1"
        ), "id");
        /*((IDictionary<string, object>)result).Add("insertId", insertId);*/
        Utils.SetProperty(result, "insertId", insertId);
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
      string table, int id, JsonElement body
    ) =>
    {
      var parameters = Utils.JSONToArray(body.ToString(), "id", id);
      var columns = "";
      for (var i = 0; i < parameters.Length; i += 2)
      {
        var column = (string)parameters[i];
        if (column.ToLower() == "id") { continue; }
        columns += (i != 0 ? ", " : "") + column + " = $" + column;
      }
      var sql = $"UPDATE {table} SET {columns} WHERE id = $id";
      return Result.encode(SQLQuery.RunOne(sql, parameters));
    });

    app.MapDelete("/api/{table}/{id}", (string table, int id) =>
      Result.encode(SQLQuery.RunOne(
        $"DELETE FROM {table} WHERE id = $id",
        "id", id
      ))
    );
  }

}