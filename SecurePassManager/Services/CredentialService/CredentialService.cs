using System.Security.Cryptography;
using SecurePassManager.Data;
using SecurePassManager.Models;
using SecurePassManager.Services.CryptographyService;

namespace SecurePassManager.Services.CredentialService
{
    public class CredentialService(AppDbContext dbContext, ICryptoService cryptoService) : ICredentialService
    {
        public void AddCredential()
        {
            Console.Write("Enter website: ");
            var website = Console.ReadLine();
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            try
            {
                // Check if the encryption key exists
                var key = cryptoService.ReadKeyFromFile();

                // Validate input data
                if (string.IsNullOrWhiteSpace(website) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Website, username, and password are required.");
                    return;
                }

                // Encrypt the password before storing
                var encryptedPassword = cryptoService.EncryptPassword(password, key);
                var encryptedPasswordBase64 = Convert.ToBase64String(encryptedPassword);

                // Create and add the new credential with the encrypted password
                var credential = new Credential
                {
                    Website = website,
                    Username = username,
                    Password = encryptedPasswordBase64
                };
                dbContext.Credentials.Add(credential);
                dbContext.SaveChanges();

                Console.WriteLine("Credential added successfully.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: Encryption key file not found.");
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Error: Failed to encrypt the password.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void  ListCredentials()
        {
            var credentials = dbContext.Credentials.ToList();
            if (credentials.Any())
            {
                Console.WriteLine($"{"ID",-5}{"Website",-20}{"Username",-20}Password");

                byte[] key;
                try
                {
                    key = cryptoService.ReadKeyFromFile();
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Error: Key file not found. Please ensure the key file exists and try again.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    return;
                }

                foreach (var cred in credentials)
                {
                    // Decrypt the password
                    string decryptedPassword;
                    try
                    {
                        decryptedPassword = cryptoService.DecryptPassword(cred.Password, key);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error decrypting password for credential with ID {cred.Id}: {ex.Message}");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"An unexpected error occurred while decrypting password for credential with ID {cred.Id}: {ex.Message}");
                        continue;
                    }

                    Console.WriteLine($"{cred.Id,-5}{cred.Website,-20}{cred.Username,-20}{decryptedPassword}");
                }
            }
            else
            {
                Console.WriteLine("No credentials found.");
            }
        }


    }
}
