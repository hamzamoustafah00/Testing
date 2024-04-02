using Newtonsoft.Json;

public static class JSON
{
    public static object Parse(string json)
    {
        object parsed;
        try
        {
            parsed = JsonConvert.DeserializeObject
                <Dictionary<string, dynamic>>(json)!;
        }
        catch (Exception)
        {
            parsed = JsonConvert.DeserializeObject
                       <Dictionary<string, dynamic>[]>(json)!;
        }
        return parsed == null ?
            new Dictionary<string, dynamic>() : parsed;
    }

    public static string Stringify(object? obj)
    {
        obj = obj != (object)"[undefined]" ? obj : null;
        return JsonConvert.SerializeObject(obj);
    }

    public static string StringifyIndented(object? obj)
    {
        obj = obj != (object)"[undefined]" ? obj : null;
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}