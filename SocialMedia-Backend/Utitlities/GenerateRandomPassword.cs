using System.Text;

namespace SocialMedia_Backend.Utitlities;

public static class GenerateRandomPassword
{

    private static readonly Random random = new Random();
    private const string charsUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string charsLower = "abcdefghijklmnopqrstuvwxyz";
    private const string digits = "0123456789";
    private const string specialChars = "!@#$%^&*()-_=+[]{}|;:',.<>?";

    public static string GenerateRandomString(int length)
    {
        var result = new StringBuilder();
        string allChars = charsUpper + charsLower + digits + specialChars;

        for (int i = 0; i < length; i++)
        {
            char randomChar = allChars[random.Next(allChars.Length)];
            result.Append(randomChar);
        }

        return result.ToString();
    }
}
