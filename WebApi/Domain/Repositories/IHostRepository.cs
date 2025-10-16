using System.Linq.Expressions;
using WebApi.Domain.Dtos;
using Host = WebApi.Domain.Entities.Host;
namespace WebApi.Domain.Repositories
{
    public interface IHostRepository
    {
       public Task<IEnumerable<Host_DTO>> GetAllAsync(int page, int pageSize);
       public Task<Host_DTO> GetByIdAsync(Guid? id);
       public Task<Host_DTO> GetByNameAsync(string? name);
       public Task AddAsync(Host property);
       public void Update(Host_DTO property);
       public void Delete(Host_DTO property);
    }
}
