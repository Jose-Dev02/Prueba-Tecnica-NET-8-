using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Entities
{
    public class DomainEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(50)]
        public required string EventType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string PayloadJSON { get; set; }

        [ForeignKey("Property")]
        public Guid PropertyId { get; set; }

        public required Property Property { get; set; }
    }
}