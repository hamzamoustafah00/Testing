using System.Dynamic;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

public class SQLQuery
{

  public static string? dbPath;

  public static SqliteConnection db =
    new SqliteConnection("Data Source=_db.sqlite3");

  public static object Run(string sql, params object[] parameters)
  {
    db.Open();

    // Prepare statement/command
    var command = db.CreateCommand();
    command.CommandText = @sql;
    for (var i = 0; i < parameters.Length; i += 2)
    {
      command.Parameters.AddWithValue((string)parameters[i], parameters[i + 1]);
    }

    // Run query and collect result
    var regExInt = new Regex("^\\d{1,}$", RegexOptions.Compiled);
    var regExDouble = new Regex("^[\\d\\.]{1,}$", RegexOptions.Compiled);
    var rows = new List<ExpandoObject>();

    try
    {
      // Non-SELECT queries
      if (sql.Split(" ")[0].ToUpper() != "SELECT")
      {
        dynamic row = new ExpandoObject();
        var rowsAffected = command.ExecuteNonQuery();
        Utils.SetProperty(row, "command", sql.Split(" ")[0].ToUpper());
        Utils.SetProperty(row, "rowsAffected", rowsAffected);
        rows.Add(row);
      }
      // SELECT queries
      else
      {
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            dynamic row = new ExpandoObject();
            for (var i = 0; i < reader.FieldCount; i++)
            {
              var valueAsStr = reader.GetString(i);
              object value = valueAsStr;
              if (regExDouble.IsMatch(valueAsStr)) { value = reader.GetDouble(i); }
              else if (regExInt.IsMatch(valueAsStr)) { value = reader.GetInt64(i); }
              Utils.SetProperty(row, reader.GetName(i), value);
            }
            rows.Add(row);
          }
        }
      }
    }
    catch (Exception error) { return new { error = error.Message.Split("'")[1] }; }
    return rows;
  }

  public static object RunOne(string sql, params object[] parameters)
  {
    var rawResult = Run(sql, parameters);
    List<ExpandoObject> result;
    ExpandoObject resultOne;
    // If not a list then we have an eror - return the raw result
    try { result = (List<ExpandoObject>)rawResult; }
    catch (Exception) { return rawResult; }
    // If nothing at index 0 - then we have no data - return null
    try { resultOne = result[0]; }
    catch (Exception) { return default!; /*null*/ }
    // Return the single row
    return resultOne;
  }

}