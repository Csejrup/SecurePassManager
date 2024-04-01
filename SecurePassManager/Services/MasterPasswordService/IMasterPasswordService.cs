namespace SecurePassManager.Services.MasterPasswordService;

public interface IMasterPasswordService
{
    string GenerateMasterPasswordHash(string masterPassword); 
    public bool VerifyMasterPassword(string inputPassword, string storedHash);
    byte[] GenerateKeyFromPassword(string password, byte[] salt, int keySize = 32);
}