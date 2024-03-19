using Microsoft.EntityFrameworkCore;
using SecurePassManager.Models;

namespace SecurePassManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Credential> Credentials { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=SecurePassManagerDb.sqlite");
    }
}