using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.RequestObjects
{
    public class BookingRequest
    {
        [Required]
        public Guid PropertyId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }
}