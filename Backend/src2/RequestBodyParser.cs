namespace WebApp;
public static class RequestBodyParser
{
    public static dynamic ReqBodyParse(string table, Obj body)
    {
        // Filter the reuest body: 
        // Always remove "id" + remove "role" in users table
        var keys = body.GetKeys().Filter(key =>
            key != "id" && (table != "users" || key != "role")
        );
        // Return parts to use when building SQL query
        return Obj(new
        {
            insertColumns = string.Join(",", keys),
            insertValues = "$" + string.Join(",$", keys),
            update = string.Join(",", keys.Select(key => $"${key}={key}"))
        });
    }
}