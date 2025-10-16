using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Dtos
{
    public class Host_DTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
