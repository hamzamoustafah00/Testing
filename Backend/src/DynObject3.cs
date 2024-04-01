using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

// Note: 
// This part of DynObject has an extra constructor customized
// for the ADO.NET SQlite driver, used in the SQLQuery class
// and the method ToQueryParams used in the REST class

public partial class DynObject
{
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