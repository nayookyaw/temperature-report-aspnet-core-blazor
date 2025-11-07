using System.Security.Cryptography;

namespace BackendAspNetCore.Utils;

public class TextUtil
{
    public static string GenerateSecureRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var byteArray = new byte[length];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(byteArray);

        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[byteArray[i] % chars.Length];
        }

        return new string(result);
    }
}