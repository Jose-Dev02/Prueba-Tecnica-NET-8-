using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using WebApi.Domain.Dtos;

namespace WebApi.Domain.Repositories
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property_DTO>> GetAllAsync(int page, int pageSize);
        Task<Property_DTO> GetByIdAsync(Guid id);
        Task AddAsync(Property_DTO property);
        Task Update(Property_DTO property);
        Task Delete(Property_DTO property);
    }
}