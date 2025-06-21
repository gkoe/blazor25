namespace MudBlazorAuthDemo.Helper
{
    using System.Security.Cryptography;
    using System.Text;

    public class AesHelper
    {
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF0123456789");  // 16 Byte Initialisierungsvektor
        public static string Encrypt(string plainText, string masterKey)
        {
            using var aes = Aes.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(masterKey[..16]);  // 16 Byte Schlüssel
            aes.Key = bytes;
            aes.IV = IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
        public static string Decrypt(string encryptedText, string masterKey)
        {
            using var aes = Aes.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(masterKey[..16]);
            aes.Key = bytes;
            aes.IV = IV;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
