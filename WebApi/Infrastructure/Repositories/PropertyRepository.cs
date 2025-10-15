using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly AppDbContext _context;

        public PropertyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllAsync(int page, int pageSize, Expression<Func<Property, bool>>? filter = null)
        {
            var query = _context.Properties.Include(p => p.Host).AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Property> GetByIdAsync(Guid id)
        {
            return await _context.Properties.Include(p => p.Host).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
        }

        public void Update(Property property)
        {
            _context.Properties.Update(property);
        }

        public void Delete(Property property)
        {
            _context.Properties.Remove(property);
        }
    }
}