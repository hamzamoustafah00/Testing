using System.Dynamic;
using Newtonsoft.Json;

public class Utils : UtilsForProps
{

  public static void Log(string label, object obj)
  {
    Console.WriteLine(
      label + " " + JsonConvert.SerializeObject(obj)
    );
  }

  public static object[] JSONToArray(
    string json, params object[] extraParameters
  )
  {
    var dict = JsonConvert.DeserializeObject
      <Dictionary<string, dynamic>>(json);
    dict = dict != null ? dict : new Dictionary<string, dynamic>();
    var list = new List<object>();
    foreach (var item in dict)
    {
      list.Add(item.Key);
      list.Add(item.Value);
    }
    foreach (var parameter in extraParameters)
    {
      list.Add(parameter);
    }
    return list.ToArray();
  }

  public static ExpandoObject JSONToExpando(string json)
  {
    var arr = JSONToArray(json);
    var obj = new ExpandoObject();
    for (var i = 0; i < arr.Length; i += 2)
    {
      Utils.SetProperty(obj, (string)arr[i], arr[i + 1]);
    }
    return obj;
  }

}