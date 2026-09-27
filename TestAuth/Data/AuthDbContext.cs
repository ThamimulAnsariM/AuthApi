using Microsoft.EntityFrameworkCore;

namespace TestAuth.Data
{
    public class AuthDbContext:DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options)
        {
            
        }
        public DbSet<TestAuth.Models.User> Users { get; set; } = null!;
    }
}
