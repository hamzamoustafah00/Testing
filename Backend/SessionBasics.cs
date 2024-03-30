using System.Dynamic;

public class SessionBasics
{

  private string? cookieValue;

  public object Retrieve(HttpContext context)
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
    Utils.Log("result", result);
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

}