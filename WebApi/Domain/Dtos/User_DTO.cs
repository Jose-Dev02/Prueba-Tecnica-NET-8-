using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Dtos
{
    public class User_DTO
    {

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
