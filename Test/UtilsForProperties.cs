using System.Dynamic;

public class UtilsForProperties
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

  public static void SetProperty(
    object obj, string key, object value
  )
  {
    // Note: obj epected to be an ExpandoObject!
    var asDict = (IDictionary<string, object?>)obj;
    if (asDict.ContainsKey(key))
    {
      asDict.Remove(key);
    }
    asDict.Add(key, value); ;
  }

}