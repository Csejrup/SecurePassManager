using System.Security.Cryptography;
using System.Text;

namespace SecurePassManager.Services.CryptographyService
{
    public class CryptoService : ICryptoService
    {
        public (byte[], byte[]) GenerateKeyAndIv()
        {
            var key = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(12);
            return (key, iv);
        }
        public byte[] GenerateKeyFromPassword(string password)
        {
            var salt = new byte[16]; 
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA512);
            return deriveBytes.GetBytes(32); 
        }
        public void GenerateAndSaveKey()
        {
            var (key, _) = GenerateKeyAndIv();
            var keyBase64 = Convert.ToBase64String(key);
            File.WriteAllText(@"key.txt", keyBase64);
        }
       
        public string GenerateStrongPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
            var res = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    var num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }

            return res.ToString();
        }

        public byte[] EncryptPassword(string password, byte[] key)
        {
            var iv = new byte[12]; 
            RandomNumberGenerator.Fill(iv);

            byte[] encryptedPassword;
            var tag = new byte[16]; 

            using (var aesGcm = new AesGcm(key))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                encryptedPassword = new byte[passwordBytes.Length];
                aesGcm.Encrypt(iv, passwordBytes, encryptedPassword, tag);
            }

            var result = new byte[iv.Length + encryptedPassword.Length + tag.Length];
            iv.CopyTo(result, 0);
            encryptedPassword.CopyTo(result, iv.Length);
            tag.CopyTo(result, iv.Length + encryptedPassword.Length);

            return result;
        }

        public string DecryptPassword(byte[] encryptedData, byte[] key)
        {
            const int ivLength = 12; 
            const int tagLength = 16; 
            var iv = new byte[ivLength];
            var tag = new byte[tagLength];
            var encryptedPassword = new byte[encryptedData.Length - ivLength - tagLength];

            Array.Copy(encryptedData, 0, iv, 0, ivLength);
            Array.Copy(encryptedData, ivLength, encryptedPassword, 0, encryptedPassword.Length);
            Array.Copy(encryptedData, encryptedData.Length - tagLength, tag, 0, tagLength);

            using var aesGcm = new AesGcm(key);
            var passwordBytes = new byte[encryptedPassword.Length];
            aesGcm.Decrypt(iv, encryptedPassword, tag, passwordBytes);
            var password = Encoding.UTF8.GetString(passwordBytes);

            return password;
        }
        
    }
}
