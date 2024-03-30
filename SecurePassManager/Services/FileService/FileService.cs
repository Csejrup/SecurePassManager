namespace SecurePassManager.Services.FileService
{
    public class FileService : IFileService
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string ReadKeyFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}