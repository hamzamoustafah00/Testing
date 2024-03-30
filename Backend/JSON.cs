using Newtonsoft.Json;

public static class JSON
{

  public static string Stringify(Object obj)
  {
    return JsonConvert.SerializeObject(obj);
  }

}