using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;

namespace CLSPhase2.Api.Extensions
{
    public static class AesExtensions
    {
        public static string DecryptAes(this string cipherText, string key, string vector)
        {
            var bytes = Convert.FromBase64String(cipherText);
            byte[] byteArray;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(vector);
                using (var decrypt = aes.CreateDecryptor())
                {
                    byteArray = decrypt.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }
            return Encoding.UTF8.GetString(byteArray);
        }

        public static string DecryptAes(this StringValues cipherText, string key, string vector)
        {
            var bytes = Convert.FromBase64String(cipherText.ToString());
            byte[] byteArray;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(vector);
                using (var decrypt = aes.CreateDecryptor())
                {
                    byteArray = decrypt.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }
            return Encoding.UTF8.GetString(byteArray);
        }

        public static string DecryptAes(this byte[] bytes, string key, string vector)
        {
            byte[] byteArray;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(vector);
                using (var decrypt = aes.CreateDecryptor())
                {
                    byteArray = decrypt.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }
            return Encoding.UTF8.GetString(byteArray);
        }

        public static byte[] EncryptAes(this string plaintext, string key, string vector)
        {
            var bytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] byteArray;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(vector);
                using (var encrypt = aes.CreateEncryptor())
                {
                    byteArray = encrypt.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }
            return byteArray;
        }
    }
}
