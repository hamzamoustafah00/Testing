using System.Text.RegularExpressions;

public static class CheckAcl
{
    private static List<DynObject>? rules;
    public static bool on;

    public static async void Start()
    {
        while (on)
        {
            UnpackRules(SQLQuery.All(
                "SELECT * FROM acl ORDER BY allow"
            ));
            // Wait one minute before reading rules again
            await Task.Delay(60000);
        }

    }

    public static void UnpackRules(List<DynObject> allRules)
    {
        foreach (var rule in allRules)
        {
            rule.Set("regexPattern", /*new Regex(*/
                @"^" + rule.GetStr("route").Replace("/", @"\/") + @"\/"
            /*)*/);
            rule.Set("userRoles", rule.GetStr("userRoles")
                .Split(",")
                .Select(x => x.Trim())
                .ToArray()
            );
        }
        rules = allRules;
    }

    public static bool Allow(
        HttpContext context, string method = "", string path = ""
    )
    {
        if (!on) { return true; }

        // Get info about the requested route and logged in user
        method = method != "" ? method : context.Request.Method;
        path = path != "" ? path : context.Request.Path;
        var user = Session.Get(context, "user");
        var userRole = user.GetStr("role").Replace("[undefined]", "visitor");
        var userEmail = user.GetStr("email").Replace("[undefined]", "");

        // Go through all acl rules to and set allowed accordingly!
        var allowed = false;

        foreach (var rule in rules!)
        {
            // Get the properties of the rule as variables
            var ruleMethod = rule.GetStr("method");
            var ruleRegexPattern = rule.GetStr("regexPattern");
            var ruleRoles = (string[])rule.Get("userRoles");
            var ruleMatch = rule.GetStr("match") == "true";
            var ruleAllow = rule.GetStr("allow") == "allow";

            // Check if role, method and path is allowed according to the rule
            var roleOk = ruleRoles.Any(x => x == userRole);
            var methodOk = method == ruleMethod || ruleMethod == "*";
            var pathOk = Regex.IsMatch(path + "/", ruleRegexPattern);
            // Note: "match" can be false - in that case we negate pathOk!
            pathOk = ruleMatch ? pathOk : !pathOk;

            // Is everything ok?
            var allOk = roleOk && methodOk && pathOk;

            // Note: We whitelist first (check all allow rules) - ORDER BY allow
            // and then we blacklist on top of that (check all disallow rules)
            allowed = ruleAllow ? allowed || allOk : allOk ? false : allowed;
        }

        var userLabel = userEmail != "" ? "the user" : "the anonymous visitor";
        userEmail = userEmail != "" ? "'" + userEmail + "' " : "";
        Debug.Log(
            "acl", $"  ACL: {allowed} for {userLabel}"
            + $" {userEmail}with the role '{userRole}'.");
        return allowed;
    }
}