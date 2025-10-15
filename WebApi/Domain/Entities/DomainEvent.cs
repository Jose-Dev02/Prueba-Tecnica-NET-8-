using System;

namespace WebApi.Domain.Entities
{
    public class DomainEvent
    {
        public Guid Id { get; set; }
        public required string EventType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}