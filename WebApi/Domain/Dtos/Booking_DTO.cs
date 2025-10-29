namespace WebApi.Domain.Dtos
{
    public class Booking_DTO
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
    }
}