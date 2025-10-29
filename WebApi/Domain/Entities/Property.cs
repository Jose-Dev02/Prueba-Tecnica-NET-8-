using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Entities
{
    public class Property
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string Location { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public required decimal PricePerNight { get; set; }

        [Required]
        public required bool Status { get; set; } 

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Host")]
        public Guid HostId { get; set; }

        [Required]
        public required Host Host { get; set; }

        public required ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
        public required ICollection<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}   