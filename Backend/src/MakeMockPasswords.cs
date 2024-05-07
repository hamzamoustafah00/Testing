namespace WebApp;
public static class MakeMockPasswords
{
    public static void Make()
    {
        var read = File.ReadAllText(FilePath("json", "mock-users.json"));
        Arr mockUsers = JSON.Parse(read);
        mockUsers = mockUsers.Map(x =>
        {
            var email = ((string)x.email).Any(char.IsDigit) ? x.email : x.email.Replace("@", "1@");
            var password = email.Substring(0, 1).ToUpper() + email.Substring(1);
            return new
            {
                ___ = x,
                email,
                password
            };
        });
        File.WriteAllText(
            FilePath("json", "sys23-mock-users-all-emails-with-digits.json"),
            JSON.Stringify(mockUsers)
        );
        mockUsers = mockUsers.Map((x, i) =>
        {
            Log("Creating with encrypted pw", i, "/", 1000);
            return new
            {
                ___ = x,
                password = Password.Encrypt(x.password)
            };
        });
        File.WriteAllText(
            FilePath("json", "sys23-mock-users-encrypted-passwords.json"),
            JSON.Stringify(mockUsers)
        );
    }
}