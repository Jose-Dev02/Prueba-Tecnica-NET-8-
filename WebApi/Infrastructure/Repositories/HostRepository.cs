using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WebApi.Domain.Dtos;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;
using Host = WebApi.Domain.Entities.Host;

namespace WebApi.Infrastructure.Repositories
{
    public class HostRepository(AppDbContext context, IMapper mapper) : IHostRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<Host_DTO>> GetAllAsync(int page, int pageSize)
        {
            var hosts = await _context.Hosts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<Host_DTO>>(hosts);
        }

        public async Task<Host_DTO> GetByIdAsync(Guid id)
        {
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.Id == id);
            return _mapper.Map<Host_DTO>(host);
        }

        public async Task<Host_DTO> GetByFullNameAsync(string name, Guid id)
        {
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.FullName == name && h.Id != id);
            return _mapper.Map<Host_DTO>(host);
        }

        public async Task<Host_DTO> AddAsync(Host host)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Hosts.AddAsync(host);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<Host_DTO>(host);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Host_DTO hostDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingEntity = await _context.Hosts.FirstOrDefaultAsync(p => p.Id == hostDto.Id) ?? throw new KeyNotFoundException($"Host with Id {hostDto.Id} not found.");

                _mapper.Map(hostDto, existingEntity);

                _context.Entry(existingEntity).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Host_DTO hostDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                bool hasProperties = _context.Properties.Any(p => p.HostId == hostDto.Id);
                if (hasProperties)
                {
                    throw new InvalidOperationException("Cannot delete the host because it has associated properties.");
                }

                var existingEntity = await _context.Hosts.FirstOrDefaultAsync(p => p.Id == hostDto.Id) ?? throw new KeyNotFoundException($"Host with Id {hostDto.Id} not found.");
                _context.Hosts.Remove(existingEntity);
               
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
