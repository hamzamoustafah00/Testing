public partial class DynObject : Dictionary<string, dynamic>
{

  protected void init(Dictionary<string, dynamic> initDict)
  {
    foreach (var item in initDict)
    {
      this[item.Key] = item.Value;
    }
  }

  public DynObject() { }

  public DynObject(object obj)
  {
    init(JSON.Parse(JSON.Stringify(obj)));
  }

  public DynObject(string json)
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

}