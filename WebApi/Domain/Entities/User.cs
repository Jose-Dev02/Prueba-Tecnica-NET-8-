using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }
    }
}