using Microsoft.EntityFrameworkCore;
using VKTest.Models;

namespace VKTest.Data
{
    public class ApplicationContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        //public DbSet<UserGroup> UserGroup { get; set; }
        //public DbSet<UserState> UserState { get; set; }

        protected readonly IConfiguration Configuration;

        public ApplicationContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost; Port = 5432; Database=VK; Username=postgres; Password=26031974yula;");
                //optionsBuilder.EnableSensitiveDataLogging();
            }
        }

    }
}
