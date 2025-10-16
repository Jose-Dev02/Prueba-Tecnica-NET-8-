using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.RequestObjects
{
    public class HostRequest
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
    }
}
