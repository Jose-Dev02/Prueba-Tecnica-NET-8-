using AutoMapper;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;
using WebApi.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.Infrastructure.Repositories
{
    public class PropertyRepository(AppDbContext context, IMapper mapper) : IPropertyRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<Property_DTO>> GetAllAsync(int page, int pageSize, Expression<Func<Property_DTO, bool>>? filter = null)
        {
            var query = _context.Properties.Include(p => p.Host).AsQueryable();

            if (filter != null)
            {
                var entityFilter = _mapper.Map<Expression<Func<Property, bool>>>(filter);
                query = query.Where(entityFilter);
            }

            var properties = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<Property_DTO>>(properties);
        }

        public async Task<Property_DTO> GetByIdAsync(Guid? id)
        {
            var property = await _context.Properties.Include(p => p.Host).FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<Property_DTO>(property);
        }

        public async Task<Property_DTO> GetByNameAsync(string? name, Guid hostId)
        {
            var property = await _context.Properties.Include(p => p.Host).FirstOrDefaultAsync(p => p.Name == name && p.HostId == hostId);
            return _mapper.Map<Property_DTO>(property);
        }

        public async Task AddAsync(Property_DTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            await _context.Properties.AddAsync(property);
        }

        public void Update(Property_DTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            var trackedEntity = _context.Properties.Local.FirstOrDefault(p => p.Id == property.Id);

            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).CurrentValues.SetValues(property);
            }
            else
            {
                _context.Properties.Attach(property);
                _context.Entry(property).State = EntityState.Modified;
            }
        }

        public void Delete(Property_DTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            var trackedEntity = _context.Properties.Local.FirstOrDefault(p => p.Id == property.Id);

            if (trackedEntity != null)
            {
                _context.Properties.Remove(trackedEntity);
            }
            else
            {
                _context.Properties.Attach(property);
                _context.Properties.Remove(property);
            }
        }
    }
}