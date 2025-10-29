using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities
{
    public class Host
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Phone]
        public required string Phone { get; set; }

        public required ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}