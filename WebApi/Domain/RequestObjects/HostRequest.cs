using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.RequestObjects
{
    public class HostRequest
    {
        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Phone]
        public required string Phone { get; set; }
    }
}
