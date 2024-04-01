using System.Security.Cryptography;
using System.Text;

namespace SecurePassManager.Services.MasterPasswordService;

public class MasterPasswordService : IMasterPasswordService
{

    public string GenerateMasterPasswordHash(string masterPassword)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(masterPassword));
        return Convert.ToBase64String(hashBytes);
    }
    public byte[] GenerateKeyFromPassword(string password, byte[] salt, int keySize = 32)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        return deriveBytes.GetBytes(keySize); 
    }
    public bool VerifyMasterPassword(string inputPassword, string storedHash)
    {
        var inputHash = GenerateMasterPasswordHash(inputPassword);
        return inputHash == storedHash;
    }
}