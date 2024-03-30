using System.Dynamic;
using Newtonsoft.Json;

public class SessionHandler
{

  private static object Retrieve()
  {
    var cookieValue = Middleware.GetCookieValue();
    var found = SQLQuery.RunOne(
      "SELECT * FROM sessions WHERE id = $id",
      "id", cookieValue != null ? cookieValue : ""
    );
    found = found != null ? found : new ExpandoObject();
    if (!Utils.HasProperty(found, "data"))
    {
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

  public static object GetValue(string key)
  {
    return Utils.GetPropertyValue(
      Utils.GetPropertyValue(Retrieve(), "data"), key
    );
  }

  public static void SetValue(string key, object value)
  {
    // Need to pack JSON here
    var retrieved = Retrieve();
    var cookieValue = Middleware.GetCookieValue();
    var data = Utils.GetPropertyValue(retrieved, "data");
    var created = Utils.GetPropertyValue(retrieved, "created");
    Utils.SetProperty(data, key, value);
    var parameters = new List<Object>()
    {
      "id", cookieValue,
      "data",  JsonConvert.SerializeObject(data)
    };
    if (created != null)
    {
      parameters.Add("created");
      parameters.Add(created);
    }
    var addCreated1 = created != null ? ", created" : "";
    var addCreated2 = created != null ? ", $created" : "";
    var sql = $"INSERT INTO sessions(id, data{addCreated1})" +
      $" VALUES($id,$data{addCreated2})";
    Console.WriteLine(sql);
    foreach (var parameter in parameters)
    {
      Console.WriteLine(parameter);
    }
    SQLQuery.RunOne(
      "DELETE FROM sessions WHERE id = $id",
      "id", cookieValue
    );
    SQLQuery.RunOne(sql, parameters.ToArray());
  }

}