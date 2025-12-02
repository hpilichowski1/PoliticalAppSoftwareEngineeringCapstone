using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.Models;

namespace PoliticalAppAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Representative> Representatives => Set<Representative>();

    }
}
