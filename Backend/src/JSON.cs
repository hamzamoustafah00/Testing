using Newtonsoft.Json;

public static class JSON
{
    public static Dictionary<string, dynamic> Parse(string json)
    {
        var parsed = JsonConvert.DeserializeObject
            <Dictionary<string, dynamic>>(json);
        return parsed == null ?
            new Dictionary<string, dynamic>() : parsed;
    }

    public static string Stringify(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}