using System.Dynamic;
using System.Text.Json;
using Newtonsoft.Json;

public class Utils
{

  public static bool HasProperty(object obj, string property)
  {
    obj = obj == null ? new { } : obj;
    return obj is ExpandoObject ?
     ((IDictionary<string, object>)obj).ContainsKey(property) :
      obj.GetType().GetProperty(property) != null;
  }

  public static object GetPropertyValue(object obj, string property)
  {
    obj = obj == null ? new { } : obj;
    var value = obj is ExpandoObject && HasProperty(obj, property) ?
    ((IDictionary<string, object>)obj)[property] :
     obj.GetType().GetProperty(property);
    return value != null ? value : default! /*null*/;
  }

  public static object[] JSONElementToArray(
    JsonElement jsonEl, params object[] extraParameters
  )
  {
    var dict = JsonConvert.DeserializeObject
      <Dictionary<string, dynamic>>(jsonEl.ToString());
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

}