
namespace SecurePassManager.Services.CryptographyService
{
    public interface ICryptoService
    {
        (byte[], byte[]) GenerateKeyAndIv();
        void GenerateAndSaveKey();
        string GenerateStrongPassword(int length);
        byte[] EncryptPassword(string password, byte[] key);
        string DecryptPassword(byte[] encryptedData, byte[] key);
        byte[] GenerateKeyFromPassword(string password);
    }
}
