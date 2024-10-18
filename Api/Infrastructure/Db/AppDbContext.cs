using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Db
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Administrator> Administrators{ get; set; } = default!;
        public DbSet<Vehicle> Vehicles{ get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator
                {
                    Id = 1,
                    Email = "administrador@teste.com",
                    Password = "123456",
                    Profile = "Adm"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                var conn = _configuration.GetConnectionString("DefaultConnection")?.ToString();
                if (!string.IsNullOrEmpty(conn))
                {
                    optionsBuilder.UseSqlServer(conn);
                }
            }
        }
    }
}