using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

// Note: 
// This part of DynObject has an extra constructor customized
// for the ADO.NET SQlite driver, used in the SQLQuery class
// and the method ToQueryParams used in the REST class

public partial class DynObject
{
    public DynObject(SqliteDataReader reader)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var key = reader.GetName(i);

            var str = reader.GetString(i);
            object value =
                Regex.IsMatch(str, @"^\d{1,}$") ? reader.GetInt64(i) :
                Regex.IsMatch(str, @"^[\d\.]{1,}$") ? reader.GetDouble(i) :
                str;
            Set(key, value);
        }
    }

    public object[] ToQueryParams()
    {
        var parameters = new List<object>();
        foreach (var item in this)
        {
            parameters.Add(item.Key);
            // Always encrypt passwords
            parameters.Add(item.Key == "password" ?
                Password.Encrypt(item.Value) : item.Value);
        }
        return parameters.ToArray();
    }
}