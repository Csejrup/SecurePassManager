using SecurePassManager.Models;

namespace SecurePassManager.Services.CredentialService;

public interface ICredentialService
{
    void AddCredential();
    void ListCredentials();
}