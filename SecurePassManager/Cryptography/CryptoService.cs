using System.Security.Cryptography;
using System.Text;

namespace SecurePassManager.Cryptography;

public static class CryptoService
    {
        // Generates a new AES-256 key and IV.
        public static (byte[], byte[]) GenerateKeyAndIv()
        {
            var key = RandomNumberGenerator.GetBytes(32); // 256 bits for AES-256
            var iv = RandomNumberGenerator.GetBytes(12); // 96 bits is standard for GCM nonce
            return (key, iv);
        }
        public static byte[] EncryptPassword(string password, byte[] key, byte[] iv)
        {
            byte[] encryptedPassword;
            var tag = new byte[16]; 

            using (var aesGcm = new AesGcm(key))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                encryptedPassword = new byte[passwordBytes.Length];
                aesGcm.Encrypt(iv, passwordBytes, encryptedPassword, tag);
            }

            // StringBuilder sb = new StringBuilder(encryptedPassword.Length * 2); // For Testing purpose
           // foreach (byte b in encryptedPassword)
             // {
              
            //    sb.AppendFormat("{0:x2}", b);
             //  }
             //  Console.WriteLine(sb.ToString()); 

            // Combine IV, encrypted password, and tag into a single array for storage.
            var result = new byte[iv.Length + encryptedPassword.Length + tag.Length];
            iv.CopyTo(result, 0);
            encryptedPassword.CopyTo(result, iv.Length);
            tag.CopyTo(result, iv.Length + encryptedPassword.Length);

            return result;
        }

        public static string DecryptPassword(byte[] encryptedDataWithIvTag, byte[] key)
        {
            var ivLength = 12;
            var tagLength = 16;
            var iv = new byte[ivLength];
            var tag = new byte[tagLength];
            var encryptedPassword = new byte[encryptedDataWithIvTag.Length - ivLength - tagLength];

            Array.Copy(encryptedDataWithIvTag, 0, iv, 0, ivLength);
            Array.Copy(encryptedDataWithIvTag, encryptedDataWithIvTag.Length - tagLength, tag, 0, tagLength);
            Array.Copy(encryptedDataWithIvTag, ivLength, encryptedPassword, 0, encryptedPassword.Length);

            var passwordBytes = new byte[encryptedPassword.Length];
            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Decrypt(iv, encryptedPassword, tag, passwordBytes);
            }

            return Encoding.UTF8.GetString(passwordBytes);
        }

    }