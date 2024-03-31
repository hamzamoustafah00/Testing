public abstract class DynObjectGeneric
{

  protected Dictionary<string, dynamic> memory;

  public DynObjectGeneric()
  {
    memory = new Dictionary<string, dynamic>();
  }

  public DynObjectGeneric(string json)
  {
    memory = JSON.Parse(json);
  }

  public DynObjectGeneric(object obj)
  {
    memory = JSON.Parse(JSON.Stringify(obj));
  }

  public void Delete(string key)
  {
    memory.Remove(key);
  }

  public void Set(string key, object value)
  {
    Delete(key);
    memory.Add(key, value);
  }

  public object Get(string key)
  {
    return memory.ContainsKey(key) ?
      memory[key] : "undefined";
  }

  public string GetStr(string key)
  {
    return Get(key) + "";
  }

  public int GetInt(string key)
  {
    var value = Get(key);
    return value is string ? 0 : Convert.ToInt32(value);
  }

  public double GetDouble(string key)
  {
    var value = Get(key);
    return value is string ? 0 : Convert.ToDouble(value);
  }

  public bool GetBool(string key)
  {
    var value = Get(key);
    var result = false;
    result = result || value is bool && (bool)value;
    result = result || value is string && (string)value != "";
    result = result || !(value is string) && GetInt(key) != 0;
    return result;
  }

  public string ToJson()
  {
    return JSON.Stringify(memory);
  }

  public override string ToString()
  {
    return ToJson();
  }

}