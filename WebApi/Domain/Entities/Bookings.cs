using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Entities
{
    public class Bookings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required DateTime CheckIn { get; set; }
        public required DateTime CheckOut { get; set; }

        [Range(0, double.MaxValue)]
        public required decimal TotalPrice { get; set; }

        [ForeignKey("Property")]
        public required Guid PropertyId { get; set; }
        public required Property Property { get; set; }
    }
}
