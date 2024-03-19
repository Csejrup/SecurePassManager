using SecurePassManager.Data;
using SecurePassManager.Models;
using SecurePassManager.Cryptography; 

var dbContext = new AppDbContext();

while (true)
{
    Console.WriteLine("1. Add New Credential");
    Console.WriteLine("2. List Credentials");
    Console.WriteLine("3. Quit");
    Console.Write("Select an option: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            // Add new credential logic
            Console.Write("Enter website: ");
            var website = Console.ReadLine();
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine(); 

            // Encrypt the password before storing
            var (key, iv) = CryptoService.GenerateKeyAndIv();
            if (password != null)
            {
                var encryptedPassword = CryptoService.EncryptPassword(password, key, iv);
                string encryptedPasswordBase64 = Convert.ToBase64String(encryptedPassword);

                // Create and add the new credential with the encrypted password
                if (website != null && username != null)
                {
                    var credential = new Credential 
                    { 
                        Website = website, 
                        Username = username, 
                        Password = encryptedPasswordBase64
                    };
                    dbContext.Credentials.Add(credential);
                }
            }

            dbContext.SaveChanges();

            Console.WriteLine("Credential added successfully.");
            break;
        case "2":
            // List credentials logic
            var credentials = dbContext.Credentials.ToList();
            if (credentials.Count != 0)
            {
                Console.WriteLine($"{"ID",-5}{"Website",-20}{"Username",-20}{"Password"}");

                byte[] key2;
                try
                {
                    var storedKeyBase64 = File.ReadAllText(@"key.txt"); 
                    key2 = Convert.FromBase64String(storedKeyBase64); 
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Error: Key file not found. Please ensure the key file exists and try again.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    break; 
                }

                foreach (var cred in credentials)
                {
                    // Convert the stored password data from Base64 to byte array
                    var encryptedDataWithIvTag = Convert.FromBase64String(cred.Password);

                    var iv2 = new byte[12];
                    var tag = new byte[16];
                    var encryptedPassword2 = new byte[encryptedDataWithIvTag.Length - iv2.Length - tag.Length];
                    Array.Copy(encryptedDataWithIvTag, 0, iv2, 0, iv2.Length);
                    Array.Copy(encryptedDataWithIvTag, iv2.Length, encryptedPassword2, 0, encryptedPassword2.Length);
                    Array.Copy(encryptedDataWithIvTag, iv2.Length + encryptedPassword2.Length, tag, 0, tag.Length);

                    var encryptedDataWithTag = new byte[encryptedPassword2.Length + tag.Length];
                    Array.Copy(encryptedPassword2, 0, encryptedDataWithTag, 0, encryptedPassword2.Length);
                    Array.Copy(tag, 0, encryptedDataWithTag, encryptedPassword2.Length, tag.Length);

                    // Decrypt the password
                    var decryptedPassword = CryptoService.DecryptPassword(encryptedDataWithTag, key2);

                    Console.WriteLine($"{cred.Id,-5}{cred.Website,-20}{cred.Username,-20}{decryptedPassword}");
                }
            }
            else
            {
                Console.WriteLine("No credentials found.");
            }
            break;

        case "3":
            // Exit the program
            return;
        
        default:
            Console.WriteLine("Invalid option, please try again.");
            break;
    }
}
