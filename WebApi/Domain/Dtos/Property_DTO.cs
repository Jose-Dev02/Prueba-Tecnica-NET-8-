

namespace WebApi.Domain.Dtos
{
    public class Property_DTO
    {
       
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Address { get; set; } = string.Empty;

            public Guid HostId { get; set; }
    }
}
