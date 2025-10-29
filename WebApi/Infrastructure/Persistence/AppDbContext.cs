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
        public DbSet<Bookings> Bookings { get; set; }
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

            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DomainEvent>()
                .HasOne(d => d.Property)
                .WithMany(p => p.DomainEvents)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configuración de valores predeterminados
            modelBuilder.Entity<Property>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<DomainEvent>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");



            // Configuración de precisión para campos decimales
            modelBuilder.Entity<Property>()
                .Property(p => p.PricePerNight)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bookings>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            // Índices
            modelBuilder.Entity<Property>()
                .HasIndex(p => p.HostId)
                .HasDatabaseName("IX_Property_HostId");

            modelBuilder.Entity<Host>()
                .HasIndex(h => h.FullName)
                .IsUnique()
                .HasDatabaseName("IX_Host_FullName");

            modelBuilder.Entity<Bookings>()
                .HasIndex(b => b.PropertyId)
                .HasDatabaseName("IX_Bookings_PropertyId");

            modelBuilder.Entity<DomainEvent>()
                .HasIndex(d => d.PropertyId)
                .HasDatabaseName("IX_DomainEvent_PropertyId");
        }
    }
}