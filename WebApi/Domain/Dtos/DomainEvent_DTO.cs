
namespace WebApi.Domain.Dtos
{
    public class DomainEvent_DTO
    {
        public required string EventType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string PayloadJSON { get; set; }
    }
}
