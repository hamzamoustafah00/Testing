namespace Backend;

public class RestExtras
{
    public string sql = "";
    public object[]? parameterArr;

    // Provide a subset of WHERE functionality + use of ORDER BY, LIMIT and OFFSET
    // from url query parameters in the get api routes
    public RestExtras(string where, string orderby, string limit, string offset)
    {
        var parameters = new List<object>();

        if (where != null)
        {
            // Split by operators (but keep them) so that we get
            // an array like this:
            // ["firstName","=","Maria","AND","lastName","!=","Smith"]
            string[] ops1 = { "!=", ">=", "<=", "=", ">", "<", "_AND_", "_OR_", "AND", "OR" };
            string[] ops2 = { "!=", ">=", "<=", "=", ">", "<", "AND", "OR", "AND", "OR" };
            foreach (var op in ops1)
            {
                where = where.Split(op).Join($"_-_{ops1.IndexOf(op)}_-_");
            }
            var parts = where.Split("_-_")
                .Select((x, i) => i % 2 == 0 ? x : ops2[Int32.Parse(x)])
                .ToArray();
            // Now we should have AND or OR on every 4:th place (n%4 = 3)
            // otherwise the syntax is incorrect!
            var i = 0;
            var faulty = parts
                .Any(x => i++ % 4 == 3 ? x != "AND" && x != "OR" : false);
            if (!faulty)
            {
                // We have keys on every n%4 = 0 place, collect those
                // and clean them so they only have safe characters
                var keys = new Queue<string>(parts
                    .Where((x, i) => i % 4 == 0)
                    .Select(x => x.Regplace(@"[^A-Za-z0-9_\-,]", ""))
                    .ToList());
                // We have values on every n%4 = 2 place, collect those
                var values = new Queue<string>(parts
                    .Where((x, i) => i % 4 == 2).ToList());
                // And operators on every n%2 = 1 place, collect those
                var operators = new Queue<string>(parts
                    .Where((x, i) => i % 2 == 1).ToList());

                // Now build the sql for where and the parameter array
                var sqlWhere = "";
                while (values.Count() > 0)
                {
                    var key = keys.Dequeue();
                    string val = values.Dequeue();
                    object value =
                        val.Match(@"^\d{1,}$") ? Int64.Parse(val + "") :
                        val.Match(@"^[\d\.]{1,}$") ? Double.Parse(val + "") :
                        val;
                    sqlWhere += $"{key} {operators.Dequeue()} ${key}";
                    sqlWhere += operators.Count() == 0 ? "" : $" {operators.Dequeue()} ";
                    parameters.Add(key);
                    parameters.Add(value);
                }

                sql += " WHERE " + sqlWhere;
            }
        }

        // Sanitize orderby and change -field to field DESC
        if (orderby != null)
        {
            orderby = orderby
                .Regplace(@"[^A-Za-z0-9_\-,]", "")
                .Split(",")
                .Select(x => x.Regplace(@"\+", "")
                    .Regplace(@"^\-(.*)", "$1 DESC")
                    .Regplace(@"\-", "")
                ).ToArray().Join(", ");
            sql += " ORDER BY " + orderby;
        }

        // Check that limit is an integer, if so use it
        var hasLimit = false;
        if (limit != null && limit.Match(@"^\d{1,}$"))
        {
            sql += " LIMIT " + limit;
            hasLimit = true;
        }

        // If we have a limit and offset is an integer, use it
        if (hasLimit && offset != null && offset.Match(@"^\d{1,}$"))
        {
            sql += " OFFSET " + offset;
        }

        parameterArr = parameters.ToArray();
    }
}