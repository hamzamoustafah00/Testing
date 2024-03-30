public class Result
{

  public static IResult encode(object result)
  {
    var statusCode =
      Utils.HasProperty(result, "error") ? 500 :
      (int?)Utils.GetPropertyValue(result, "rowsAffected") == 0 ? 404 :
      result == null ? 404 :
      200;

    result = result != null ? result : new { error = "404. Not found." };

    return Results.Json(
      result,
      new System.Text.Json.JsonSerializerOptions(),
      "text/json",
      statusCode
    );
  }

}