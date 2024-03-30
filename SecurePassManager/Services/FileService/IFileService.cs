
namespace SecurePassManager.Services.FileService
{
    public interface IFileService
    {
        bool FileExists(string filePath);
        string ReadKeyFromFile(string filePath);
    }
}
