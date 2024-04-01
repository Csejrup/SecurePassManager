namespace SecurePassManager.Models;

public class Credential
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; } 
    public required string Website { get; set; }
    
    public required string MasterPasswordHashed { get; set; }
}