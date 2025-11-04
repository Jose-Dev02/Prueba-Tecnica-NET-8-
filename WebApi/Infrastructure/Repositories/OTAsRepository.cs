using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApi.Domain.Dtos;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Infrastructure.Repositories
{
    public class OTAsRepository(AppDbContext context, IMapper mapper) : IOTAsRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<DomainEvent_DTO?> Sync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var property = await _context.Properties.FindAsync(id);
                if (property != null)
                {

                    var domainEvent = new DomainEvent
                    {
                        EventType = "SyncWithOTAs",
                        CreatedAt = DateTime.UtcNow,
                        Property = property,
                        PropertyId = id,
                        PayloadJSON = JsonSerializer.Serialize(new
                        {
                            ota = "Booking.com",
                            status = "completed"

                        }),
                    };
                    _context.DomainEvents.Add(domainEvent);
                    return _mapper.Map<DomainEvent_DTO>(domainEvent);
                }

                return null;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
    }
}
