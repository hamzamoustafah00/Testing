namespace Backend;

public static class Debug
{
    public static bool on = false;

    public static void Log(string type, object data)
    {
        return;
        if (!on) { return; }

        if (type == "route")
        {
            var req = ((HttpContext)data).Request;
            var route = req.Method + " " + req.Path
                + HttpUtility.UrlDecode(req.QueryString + "");
            Console.WriteLine("\n" + route);
        }

        if (type == "acl")
        {
            Console.WriteLine(data);
        }

        if (type == "sql")
        {
            var d = new DynObject(data);
            var sql = Regex.Replace(d.GetStr("sql"), @"\s+", " ");

            // Ignore logging for: 
            // 1) Sql queries to session and acl
            // 2) The SELECT to determine insertId after an INSERT
            if (
                sql.Contains("FROM sessions") ||
                sql.Contains("INTO sessions") ||
                sql.Contains("UPDATE sessions") ||
                sql.Contains("FROM acl") ||
                sql.Contains("__insertId")
            ) { return; }

            Console.WriteLine("  " + sql);
            var p = ((Newtonsoft.Json.Linq.JArray)
                d.Get("parameters")).ToArray();
            var longestKey = p
                .Select((x, i) => i % 2 == 1 ? 0 : ((string?)x)!.Length)
                .ToArray().DefaultIfEmpty(0).Max(x => x);
            for (var i = 0; i < p.Length; i += 2)
            {
                var key = ((string?)p[i])!.PadRight(longestKey);
                var val = p[i + 1];
                val = val.Type + "" == "String" ? $"\"{val}\"" : val;
                Console.WriteLine($"    ${key} =  {val}");
            }
        }

        if (type == "sqlError")
        {
            Console.WriteLine("  SQL Error: " + data);
        }
    }
}