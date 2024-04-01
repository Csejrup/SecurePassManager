using System;
using SecurePassManager.Data;
using SecurePassManager.Services.CryptographyService;
using SecurePassManager.Handlers.MenuHandler;
using SecurePassManager.Services.CredentialService;
using SecurePassManager.Services.FileService;
using SecurePassManager.Services.MasterPasswordService;

namespace SecurePassManager
{
    public class Program(IMenuHandler menuHandler)
    {
        private void Run()
        {
            menuHandler.RunMenuLoop();
        }

        public static void Main(string[] args)
        {
            var dbContext = new AppDbContext();
            var cryptoService = new CryptoService();
            var fileService = new FileService();
            var masterPasswordService = new MasterPasswordService();
            var credentialService = new CredentialService(dbContext,  masterPasswordService,cryptoService);
            var menuHandler = new MenuHandler( cryptoService, fileService, credentialService );

            var program = new Program(menuHandler);
            program.Run();
        }
    }
}