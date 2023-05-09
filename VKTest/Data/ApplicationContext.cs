using Microsoft.EntityFrameworkCore;
using VKTest.Models;

namespace VKTest.Data
{
    public class ApplicationContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserState> UserStates { get; set; }

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connection = "Host=localhost; Port = 5432; Database=VK; Username=postgres; Password=26031974yula;";
                optionsBuilder.UseNpgsql(connection);
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

    }
}
