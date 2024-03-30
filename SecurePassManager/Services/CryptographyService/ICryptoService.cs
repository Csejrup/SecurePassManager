
namespace SecurePassManager.Services.CryptographyService
{
    public interface ICryptoService
    {
        (byte[], byte[]) GenerateKeyAndIv();
        void GenerateAndSaveKey();
        string GenerateStrongPassword(int length);
        byte[] EncryptPassword(string password, byte[] key);
        string DecryptPassword(string password, byte[] key);
        byte[] HashDataUsingSha512(string data);
        byte[] CreateHmacSha256(byte[] data, byte[] key);

        byte[] ReadKeyFromFile();
    }
}
