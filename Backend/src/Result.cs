public static class Result
{
    private static DynObject rowModifier(DynObject row)
    {
        // If null value then change the result to reflect that
        row = row != null ? row :
            new DynObject(new { error = "Not found." });
        // Delete the password
        row.Delete("password");
        // JSON.parse all fields called "data"
        if (row.HasKey("data"))
        {
            row.Set("data", JSON.Parse(row.GetStr("data")));
        }
        return row;
    }

    public static IResult encode(object result)
    {
        int statusCode = 200;

        // The results is a single row/object or an error object
        if (result is DynObject || result == null)
        {
            DynObject r = (DynObject?)result!;

            // 500 = Internal Srver Error
            // 404 = Not found
            // 200 = OK
            statusCode =
                result == null ? 404 :
                r.HasKey("error") ? 500 :
                r.HasKey("rowsAffected") && r.GetInt("rowsAffected") == 0 ? 404 :
                200;

            result = rowModifier(r);
        }

        // The result is a list
        else if (result is List<DynObject>)
        {
            var r = (List<DynObject>)result;
            result = r.Select(x => rowModifier(x));
        }

        return Results.Text(
          JSON.Stringify(result),
          "text/json",
          null,
          statusCode
        );
    }
}