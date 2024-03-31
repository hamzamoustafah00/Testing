using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;

// Note: This version of DynObject is customized 
// for SQLite and our current SQLQuery class 

public class DynObject : DynObjectGet
{
  public DynObject() : base() { }
  public DynObject(object obj) : base(obj) { }
  public DynObject(string json) : base(json) { }

  private static Regex regExInt =
    new Regex("^\\d{1,}$", RegexOptions.Compiled);
  private static Regex regExDouble =
    new Regex("^[\\d\\.]{1,}$", RegexOptions.Compiled);

  public DynObject(SqliteDataReader reader)
  {
    for (var i = 0; i < reader.FieldCount; i++)
    {
      var valueAsStr = reader.GetString(i);
      object value =
        regExDouble.IsMatch(valueAsStr) ? reader.GetDouble(i) :
        regExInt.IsMatch(valueAsStr) ? reader.GetInt64(i) :
        valueAsStr;
      Set(reader.GetName(i), value);
    }
  }

  public object[] ToQueryParams()
  {
    var parameters = new List<object>();
    foreach (var item in this)
    {
      parameters.Add(item.Key);
      parameters.Add(item.Value);
    }
    return parameters.ToArray();
  }

}