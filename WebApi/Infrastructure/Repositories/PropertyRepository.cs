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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingEntity = await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyDto.Id) ?? throw new KeyNotFoundException($"Property with Id {propertyDto.Id} not found.");

                _mapper.Map(propertyDto, existingEntity);

                _context.Entry(existingEntity).State = EntityState.Modified;

                var registerEvent = new DomainEvent
                {
                    PropertyId = existingEntity.Id,
                    EventType = "PropertyUpdated",
                    PayloadJSON = JsonSerializer.Serialize(new
                    {
                        propertyId = existingEntity.Id,
                        name = existingEntity.Name,
                        location = existingEntity.Location,
                        status = existingEntity.Status ? "Active" : "Inactive",
                        pricePerNight = existingEntity.PricePerNight
                    }),
                    Property = existingEntity
                };

                await _context.DomainEvents.AddAsync(registerEvent);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Property_DTO propertyDto)
        {
            var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyDto.Id);

            if (property == null)
            {
                throw new KeyNotFoundException($"Property with Id {propertyDto.Id} not found.");
            }

            var hasActiveBookings = await _context.Bookings.AnyAsync(b => b.PropertyId == property.Id && b.CheckOut > DateTime.UtcNow);
            if (hasActiveBookings)
            {
                throw new InvalidOperationException("Cannot delete property with active bookings.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Properties.Remove(property);

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
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}