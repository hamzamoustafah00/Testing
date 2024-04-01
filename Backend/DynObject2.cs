public partial class DynObject
{

  public bool HasKey(string key)
  {
    return Get(key) + "" != "[undefined]";
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