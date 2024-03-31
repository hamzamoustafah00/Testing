using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;

// Note: This version of DynObject is customized for SQLite
public class DynObject : DynObjectGeneric
{

  private static Regex regExInt =
    new Regex("^\\d{1,}$", RegexOptions.Compiled);
  private static Regex regExDouble =
    new Regex("^[\\d\\.]{1,}$", RegexOptions.Compiled);

  public DynObject() : base() { }

  public DynObject(string json) : base(json) { }

  public DynObject(object obj) : base(obj) { }

  public DynObject(SqliteDataReader reader)
  {
    memory = new Dictionary<string, dynamic>();
    for (var i = 0; i < reader.FieldCount; i++)
    {
      var valueAsStr = reader.GetString(i);
      object value = valueAsStr;
      if (regExDouble.IsMatch(valueAsStr)) { value = reader.GetDouble(i); }
      else if (regExInt.IsMatch(valueAsStr)) { value = reader.GetInt64(i); }
      Set(reader.GetName(i), value);
    }
  }

  public void ToSqliteParams(SqliteParameterCollection parameters)
  {
    foreach (var item in memory)
    {
      parameters.AddWithValue(item.Key, item.Value);
    }
  }

}