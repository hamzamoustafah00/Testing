public abstract class DynObjectGeneric : Dictionary<string, dynamic>
{

  protected void init(Dictionary<string, dynamic> initDict)
  {
    foreach (var item in initDict)
    {
      this[item.Key] = item.Value;
    }
  }

  public DynObjectGeneric() { }

  public DynObjectGeneric(object obj)
  {
    init(JSON.Parse(JSON.Stringify(obj)));
  }

  public DynObjectGeneric(string json)
  {
    init(JSON.Parse(json));
  }

  public void Delete(string key)
  {
    this.Remove(key);
  }

  public void Set(string key, object value)
  {
    Delete(key);
    this.Add(key, value);
  }

  public bool HasKey(string key)
  {
    return Get(key) + "" != "[undefined]";
  }

  public object Get(string key)
  {
    object obj = this;
    foreach (var part in key.Split("."))
    {
      try { obj = new DynObject(((DynObject)obj)[part]); }
      catch (Exception)
      {
        try { obj = ((DynObject)obj)[part]; }
        catch (Exception)
        {
          try
          {
            var array = ((Newtonsoft.Json.Linq.JArray)obj).ToArray();
            var index = Convert.ToInt32(part);
            obj = array[index];
          }
          catch (Exception) { obj = "[undefined]"; }
        }
      }
    }
    return obj;
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

  public string[] GetKeys()
  {
    return Keys.ToArray();
  }

  public object[] GetValues()
  {
    return Values.ToArray();
  }

  public string ToJson()
  {
    return JSON.Stringify(this);
  }

  public override string ToString()
  {
    return ToJson();
  }

}