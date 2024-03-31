public class SessionBasics
{

  private string? cookieValue;

  public DynObject Retrieve(HttpContext context)
  {
    cookieValue = cookieValue != null ?
      cookieValue : Middleware.GetCookieValue(context);
    var found = SQLQuery.RunOne(
      "SELECT * FROM sessions WHERE id = $id",
      "id", cookieValue
    );

    found = found != null ? found : new DynObject();
    if (found.HasKey("id"))
    {
      found.Set("id", cookieValue);
      found.Set("data", new DynObject());
    }
    else
    {
      found.Set("data", new DynObject(found.Get("data")));
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
    if (result.GetInt("rowsAffected") == 0)
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