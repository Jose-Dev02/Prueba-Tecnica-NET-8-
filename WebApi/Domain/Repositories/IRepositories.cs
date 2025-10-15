using WebApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace WebApi.Domain.Repositories
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllAsync(int page, int pageSize, Expression<Func<Property, bool>>? filter = null);
        Task<Property> GetByIdAsync(Guid id);
        Task AddAsync(Property property);
        void Update(Property property);
        void Delete(Property property);
    }
}