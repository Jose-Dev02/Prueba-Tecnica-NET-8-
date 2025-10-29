using System.ComponentModel.DataAnnotations;
using Host = WebApi.Domain.Entities.Host;

namespace WebApi.Domain.RequestObjects
{
    public class PropertyRequest
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required] 
        [MaxLength(200)]
        public required string Location { get; set; }

        [Required]
        public Guid HostId { get; set; }

    }
}
