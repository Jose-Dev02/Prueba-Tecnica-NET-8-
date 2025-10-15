using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Entities
{
    public class Property
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string Address { get; set; }

        [ForeignKey("Host")]
        public Guid HostId { get; set; }
        [Required]
        public required Host Host { get; set; }
    }
}