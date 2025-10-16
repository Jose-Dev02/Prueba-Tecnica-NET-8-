using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using WebApi.Domain.Dtos;

namespace WebApi.Domain.Repositories
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property_DTO>> GetAllAsync(int page, int pageSize, Expression<Func<Property_DTO, bool>>? filter = null);
        Task<Property_DTO> GetByIdAsync(Guid? id);
        Task<Property_DTO> GetByNameAsync(string? name, Guid hostId);
        Task AddAsync(Property_DTO property);
        void Update(Property_DTO property);
        void Delete(Property_DTO property);
    }
}