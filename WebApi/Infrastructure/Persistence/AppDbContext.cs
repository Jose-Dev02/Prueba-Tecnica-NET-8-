using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using Host = WebApi.Domain.Entities.Host;


namespace WebApi.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Property> Properties { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<User> Users { get; set; }
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

            modelBuilder.Entity<Host>()
                .HasIndex(h => h.Name)
                .IsUnique()
                .HasDatabaseName("IX_Host_Name");
        }
    }
}