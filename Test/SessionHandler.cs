using System.Dynamic;
using Newtonsoft.Json;

public class SessionHandler
{

  private string? cookieValue;

  private object Retrieve(HttpContext context)
  {
    cookieValue = cookieValue != null ?
      cookieValue : Middleware.GetCookieValue(context);
    var found = SQLQuery.RunOne(
      "SELECT * FROM sessions WHERE id = $id",
      "id", cookieValue
    );
    found = found != null ? found : new ExpandoObject();
    if (!Utils.HasProperty(found, "id"))
    {
      Utils.SetProperty(found, "id", cookieValue);
      Utils.SetProperty(found, "data", new ExpandoObject());
    }
    else
    {
      Utils.SetProperty(
        found, "data",
        Utils.JSONToExpando(
          (string)Utils.GetPropertyValue(found, "data")
        )
      );
    }
    return found;
  }

  public void Touch(HttpContext context)
  {
    cookieValue = cookieValue != null ?
      cookieValue : Middleware.GetCookieValue(context);
    var result = SQLQuery.RunOne(
      @"UPDATE sessions SET modified=DATETIME('now')
        WHERE id = $id",
      "id", cookieValue
    );
    if ((int)Utils.GetPropertyValue(result, "rowsAffected") == 0)
    {
      SQLQuery.RunOne(
        @"INSERT INTO sessions(id, data)
        VALUES ($id, $data)",
        "id", cookieValue,
        "data", "{}"
      );
    }
  }

  public object GetValue(
    HttpContext context, string key
  )
  {
    return Utils.GetPropertyValue(
      Utils.GetPropertyValue(Retrieve(context), "data"), key
    );
  }

  public void SetValue(
    HttpContext context, string key, object value
  )
  {
    var retrieved = Retrieve(context);
    var data = Utils.GetPropertyValue(retrieved, "data");
    var created = Utils.GetPropertyValue(retrieved, "created");
    Utils.SetProperty(data, key, value);
    var parameters = new List<Object>()
      {"data",  JsonConvert.SerializeObject(data)};
    string sql;
    var cookieValue = Utils.GetPropertyValue(retrieved, "id");
    parameters.Add("id");
    parameters.Add(cookieValue);
    if (created != null)
    {
      sql = @"UPDATE sessions 
              SET modified=DATETIME('now'), data=$data 
              WHERE id=$id";
    }
    else
    {
      sql = @"INSERT INTO sessions(id, data) 
              VALUES($id,$data)";
    }
    SQLQuery.RunOne(sql, parameters.ToArray());
  }

}