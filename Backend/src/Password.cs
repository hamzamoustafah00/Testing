using BCryptNet = BCrypt.Net.BCrypt;

static class Password
{
    private static int cost = 13;

    public static string Encrypt(string password)
    {
        return BCryptNet.EnhancedHashPassword(password, workFactor: cost);
    }

    public static bool Verify(string password, string encrypted)
    {
        return BCryptNet.EnhancedVerify(password, encrypted);
    }
}