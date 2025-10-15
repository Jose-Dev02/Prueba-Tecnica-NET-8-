using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities
{
    public class Host
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public required ICollection<Property> Properties { get; set; }
    }
}