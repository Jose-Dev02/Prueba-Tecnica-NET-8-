using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using HostEntity = WebApi.Domain.Entities.Host;
using UserEntity = WebApi.Domain.Entities.User;

namespace WebApi.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<HostEntity> Hosts { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<DomainEvent> DomainEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones y restricciones
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Host)
                .WithMany(h => h.Properties)
                .HasForeignKey(p => p.HostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            modelBuilder.Entity<Property>()
                .HasIndex(p => p.HostId)
                .HasDatabaseName("IX_Property_HostId");
        }
    }
}