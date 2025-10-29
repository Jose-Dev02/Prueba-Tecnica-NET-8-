using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using WebApi.Domain.Dtos;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Infrastructure.Repositories
{
    public class PropertyRepository(AppDbContext context, IMapper mapper) : IPropertyRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<Property_DTO>> GetAllAsync(int page, int pageSize)
        {
            var properties = await _context.Properties.Include(p => p.Host)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<Property_DTO>>(properties);
        }

        public async Task<Property_DTO> GetByIdAsync(Guid id)
        {
            var property = await _context.Properties.Include(p => p.Host).FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<Property_DTO>(property);
        }

        public async Task AddAsync(Property_DTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Properties.AddAsync(property);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var registerEvent = new DomainEvent
            {
                PropertyId = property.Id,
                EventType = "PropertyCreated",
                PayloadJSON = JsonSerializer.Serialize(new
                {
                    propertyId = property.Id,
                    name = property.Name,
                    location = property.Location,
                    status = property.Status ? "Active" : "Inactive",
                    pricePerNight = property.PricePerNight

                }),
                Property = property,
            };

            await _context.DomainEvents.AddAsync(registerEvent);
        }

        public async Task Update(Property_DTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var registerEvent = new DomainEvent
            {
                PropertyId = property.Id,
                EventType = "PropertyUpdated",
                PayloadJSON = JsonSerializer.Serialize(new
                {
                    propertyId = property.Id,
                    name = property.Name,
                    location = property.Location,
                    status = property.Status ? "Active" : "Inactive",
                    pricePerNight = property.PricePerNight

                }),
                Property = property,
            };

            await _context.DomainEvents.AddAsync(registerEvent);
        }

        public async Task Delete(Property_DTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var registerEvent = new DomainEvent
            {
                PropertyId = property.Id,
                EventType = "PropertyDeleted",
                PayloadJSON = JsonSerializer.Serialize(new
                {
                    propertyId = property.Id,
                    name = property.Name,
                    location = property.Location,
                    status = property.Status ? "Active" : "Inactive",
                    pricePerNight = property.PricePerNight

                }),
                Property = property,
            };

            await _context.DomainEvents.AddAsync(registerEvent);
        }
    }
}