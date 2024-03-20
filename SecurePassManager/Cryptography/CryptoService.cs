using System;
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

    public static void GenerateAndSaveKey()
    {
        var (key, _) = GenerateKeyAndIv();
        var keyBase64 = Convert.ToBase64String(key);       
        File.WriteAllText(@"key.txt", keyBase64);
    }
    public static string GenerateStrongPassword(int length)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        StringBuilder res = new StringBuilder();
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
    
    public static byte[] EncryptPassword(string password, byte[] key, byte[] iv)
    {
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

    public static string DecryptPassword(byte[] encryptedDataWithIvTag, byte[] key)
    {
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

    // Hashes data using SHA-512.
    public static IEnumerable<byte> HashDataUsingSha512(string data)
    {
        using var sha512 = SHA512.Create();
        return sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    // Creates a HMAC using SHA-256.
    public static byte[] CreateHmacSha256(byte[] data, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }
}
