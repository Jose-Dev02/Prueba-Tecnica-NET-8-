using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.RequestObjects
{
    public class UserRequest
    {

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
