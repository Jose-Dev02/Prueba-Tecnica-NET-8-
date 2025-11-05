using System.Linq.Expressions;
using WebApi.Domain.Dtos;
using Host = WebApi.Domain.Entities.Host;
namespace WebApi.Domain.Repositories
{
    public interface IHostRepository
    {
       public Task<IEnumerable<Host_DTO>> GetAllAsync(int page, int pageSize);
       public Task<Host_DTO> GetByIdAsync(Guid id);
       public Task<Host_DTO> AddAsync(Host property);
       public Task UpdateAsync(Host_DTO property);
       public Task DeleteAsync(Host_DTO property);
       public Task<Host_DTO> GetByFullNameAsync(string fullName, Guid id);
    }
}
