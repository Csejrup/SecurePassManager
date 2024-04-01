using System.Text;
using SecurePassManager.Models;
using SecurePassManager.Data;
using SecurePassManager.Services.CryptographyService;
using SecurePassManager.Services.MasterPasswordService;

namespace SecurePassManager.Services.CredentialService
{
    public class CredentialService(
        AppDbContext dbContext,
        IMasterPasswordService masterPasswordService,
        ICryptoService cryptoService) : ICredentialService
    {
        public void AddCredential()
        {
            Console.Write("Enter website: ");
            var website = Console.ReadLine();
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            Console.Write("Enter master password: ");
            var masterPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(website) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(masterPassword))
            {
                Console.WriteLine("All fields are required.");
                return;
            }

            var masterPasswordHashed = masterPasswordService.GenerateMasterPasswordHash(masterPassword);
            var salt = Encoding.UTF8.GetBytes(masterPasswordHashed).Take(16).ToArray(); 
            var key = masterPasswordService.GenerateKeyFromPassword(masterPassword, salt, 32);

            var encryptedPassword =cryptoService. EncryptPassword(password, key);
            var encryptedPasswordBase64 = Convert.ToBase64String(encryptedPassword);

            var credential = new Credential
            {
                Website = website,
                Username = username,
                Password = encryptedPasswordBase64,
                MasterPasswordHashed = masterPasswordHashed
            };
            dbContext.Credentials.Add(credential);
            dbContext.SaveChanges();

            Console.WriteLine("Credential added successfully.");
        }

        public void ListCredentials()
        {
            Console.Write("Enter your username: ");
            var username = Console.ReadLine();
            Console.Write("Enter master password: ");
            var masterPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(masterPassword))
            {
                Console.WriteLine("Username and master password are required.");
                return;
            }
            var userCredential = dbContext.Credentials.FirstOrDefault(c => c.Username == username);
            if (userCredential == null)
            {
                Console.WriteLine("No credentials found for the provided username.");
                return;
            }
            if (!masterPasswordService.VerifyMasterPassword(masterPassword, userCredential.MasterPasswordHashed))
            {
                Console.WriteLine("Invalid master password.");
                return;
            }
            var salt = Encoding.UTF8.GetBytes(userCredential.MasterPasswordHashed).Take(16).ToArray(); 
            var key = masterPasswordService.GenerateKeyFromPassword(masterPassword, salt, 32);
            var encryptedData = Convert.FromBase64String(userCredential.Password);
            var password = cryptoService.DecryptPassword(encryptedData, key);

            Console.WriteLine($"Website: {userCredential.Website}, Username: {userCredential.Username}, Password: {password}");
        }


       
    }
}
