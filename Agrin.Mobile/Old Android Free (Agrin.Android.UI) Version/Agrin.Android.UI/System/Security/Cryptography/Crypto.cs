using System;
using System.IO;
using System.Text;

namespace System.Security.Cryptography
{
    internal static class Crypto
    {
        private static readonly byte[] salt = Encoding.ASCII.GetBytes("Xamarin.Android Version: 4.17.0");

        internal static string Decrypt(string encryptedText, string encryptionPassword)
        {
            byte[] buffer;
            RijndaelManaged algorithm = GetAlgorithm(encryptionPassword);
            if ((encryptedText == null) || (encryptedText == string.Empty))
            {
                return string.Empty;
            }
            using (ICryptoTransform transform = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                buffer = InMemoryCrypt(Convert.FromBase64String(encryptedText), transform);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        internal static string Encrypt(string textToEncrypt, string encryptionPassword)
        {
            byte[] buffer;
            RijndaelManaged algorithm = GetAlgorithm(encryptionPassword);
            if ((textToEncrypt == null) || (textToEncrypt == string.Empty))
            {
                return string.Empty;
            }
            using (ICryptoTransform transform = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                buffer = InMemoryCrypt(Encoding.UTF8.GetBytes(textToEncrypt), transform);
            }
            return Convert.ToBase64String(buffer);
        }

        private static RijndaelManaged GetAlgorithm(string encryptionPassword)
        {
            Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(encryptionPassword, salt);
            RijndaelManaged managed = new RijndaelManaged();
            int cb = managed.KeySize / 8;
            int num2 = managed.BlockSize / 8;
            managed.Key = bytes.GetBytes(cb);
            managed.IV = bytes.GetBytes(num2);
            return managed;
        }

        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            MemoryStream stream = new MemoryStream();
            using (Stream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                stream2.Write(data, 0, data.Length);
            }
            return stream.ToArray();
        }
    }
}

