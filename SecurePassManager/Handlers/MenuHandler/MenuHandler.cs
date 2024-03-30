using SecurePassManager.Services.CryptographyService;
using SecurePassManager.Services.CredentialService;
using SecurePassManager.Services.FileService;

namespace SecurePassManager.Handlers.MenuHandler
{
    public class MenuHandler(ICryptoService cryptoService, IFileService fileService, ICredentialService credentialsService)
        : IMenuHandler
    {
        public void Initialize()
        {
            // Check if the encryption key exists, generate and save if not
            const string keyFilePath = @"key.txt";
            if (!fileService.FileExists(keyFilePath))
            {
                Console.WriteLine("Encryption key file not found, generating a new one...");
                cryptoService.GenerateAndSaveKey();
                Console.WriteLine("New encryption key generated and saved.");
            }
            else
            {
                Console.WriteLine("Using existing encryption key.");
            }
        }

        public void RunMenuLoop()
        {
            while (true)
            {
                DisplayMenu();
                var option = Console.ReadLine();

                // Validate user input for menu option
                if (string.IsNullOrWhiteSpace(option) || !int.TryParse(option, out var choice))
                {
                    Console.WriteLine("Invalid option. Please enter a valid numeric option.");
                    continue;
                }

                // Process user input
                ProcessOption(option);
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("1. Add New Credential");
            Console.WriteLine("2. List Credentials");
            Console.WriteLine("3. Generate Strong Password");
            Console.WriteLine("4. Quit");
            Console.Write("Select an option: ");
        }

        private void ProcessOption(string option)
        {
            if (string.IsNullOrWhiteSpace(option))
            {
                Console.WriteLine("Invalid option, please try again.");
                return;
            }

            switch (option)
            {
                case "1":
                    credentialsService.AddCredential();
                    break;
                case "2":
                    credentialsService.ListCredentials();
                    break;
                case "3":
                    GenerateStrongPassword();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }


        private void GenerateStrongPassword()
        {
            Console.Write("Enter the desired password length (default is 16): ");
            var input = Console.ReadLine();
            var length = 16;
            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int customLength))
            {
                length = customLength;
            }

            string strongPassword = cryptoService.GenerateStrongPassword(length);
            Console.WriteLine($"Generated Password: {strongPassword}");
        }
    }
}