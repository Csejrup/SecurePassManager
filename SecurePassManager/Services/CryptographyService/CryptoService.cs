using System.Security.Cryptography;
using System.Text;

namespace SecurePassManager.Services.CryptographyService
{
    public class CryptoService : ICryptoService
    {
        public (byte[], byte[]) GenerateKeyAndIv()
        {
            var key = RandomNumberGenerator.GetBytes(32); // 256 bits for AES-256
            var iv = RandomNumberGenerator.GetBytes(12); // 96 bits is standard for GCM nonce
            return (key, iv);
        }

        public void GenerateAndSaveKey()
        {
            var (key, _) = GenerateKeyAndIv();
            var keyBase64 = Convert.ToBase64String(key);
            File.WriteAllText(@"key.txt", keyBase64);
        }
        public byte[] ReadKeyFromFile()
        {
            try
            {
                var storedKeyBase64 = File.ReadAllText(@"key.txt");
                return Convert.FromBase64String(storedKeyBase64);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Encryption key file not found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while reading the key file: {ex.Message}");
            }
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
            var tag = new byte[16]; // GCM tag length is 128 bits (16 bytes)

            using (var aesGcm = new AesGcm(key))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                encryptedPassword = new byte[passwordBytes.Length];
                aesGcm.Encrypt(iv, passwordBytes, encryptedPassword, tag);
            }

            // Combine IV, encrypted password, and tag into a single array for storage.
            var result = new byte[iv.Length + encryptedPassword.Length + tag.Length];
            iv.CopyTo(result, 0);
            encryptedPassword.CopyTo(result, iv.Length);
            tag.CopyTo(result, iv.Length + encryptedPassword.Length);

            return result;
        }


        public string DecryptPassword(string password, byte[] key)
        {
            var encryptedDataWithIvTag = Convert.FromBase64String(password);

            const int ivLength = 12; 
            const int tagLength = 16; 
            if (encryptedDataWithIvTag.Length < ivLength + tagLength)
            {
                throw new ArgumentException("Invalid encrypted data length.");
            }

            var iv = new byte[ivLength];
            var tag = new byte[tagLength];
            var encryptedPassword = new byte[encryptedDataWithIvTag.Length - ivLength - tagLength];

            Array.Copy(encryptedDataWithIvTag, 0, iv, 0, ivLength);
            Array.Copy(encryptedDataWithIvTag, ivLength, encryptedPassword, 0, encryptedPassword.Length);
            Array.Copy(encryptedDataWithIvTag, encryptedDataWithIvTag.Length - tagLength, tag, 0, tagLength);

            var passwordBytes = new byte[encryptedPassword.Length];

            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Decrypt(iv, encryptedPassword, tag, passwordBytes);
            }
            return Encoding.UTF8.GetString(passwordBytes);
        }


        public byte[] HashDataUsingSha512(string data)
        {
            return SHA512.HashData(Encoding.UTF8.GetBytes(data));
        }

        public byte[] CreateHmacSha256(byte[] data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(data);
        }
    }
}
