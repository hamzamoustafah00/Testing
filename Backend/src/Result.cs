public static class Result
{
    public static IResult encode(object result)
    {
        int statusCode = 200;

        // set the status code correctly
        if (result is DynObject)
        {
            DynObject r = (DynObject)result;

            // 500 = Internal Srver Error
            // 404 = Not found
            // 200 = OK
            statusCode =
                r.HasKey("error") ? 500 :
                r.HasKey("rowsAffected") && r.GetInt("rowsAffected") == 0 ? 404 :
                result == null ? 404 :
                200;
        }

        return Results.Json(
          result,
          new System.Text.Json.JsonSerializerOptions(),
          "text/json",
          statusCode
        );
    }
}