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

        public async Task AddAsync(Host host)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Hosts.AddAsync(host);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public void Update(Host_DTO hostDto)
        {
            var host = _mapper.Map<Host>(hostDto);
            var trackedEntity = _context.Hosts.Local.FirstOrDefault(h => h.Id == host.Id);

            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).CurrentValues.SetValues(host);
            }
            else
            {
                _context.Hosts.Attach(host);
                _context.Entry(host).State = EntityState.Modified;
            }
        }

        public void Delete(Host_DTO hostDto)
        {
            var host = _mapper.Map<Host>(hostDto);

            bool hasProperties = _context.Properties.Any(p => p.HostId == host.Id);
            if (hasProperties)
            {
                throw new InvalidOperationException("Cannot delete the host because it has associated properties.");
            }

            var trackedEntity = _context.Hosts.Local.FirstOrDefault(h => h.Id == host.Id);

            if (trackedEntity != null)
            {
                _context.Hosts.Remove(trackedEntity);
            }
            else
            {
                _context.Hosts.Attach(host);
                _context.Hosts.Remove(host);
            }
        }

        public async Task DeleteAsync(Guid hostId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var host = await _context.Hosts.FindAsync(hostId);
                if (host != null)
                {
                    _context.Hosts.Remove(host);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Host_DTO> GetByFullNameAsync(string name)
        {
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.FullName == name);
            return _mapper.Map<Host_DTO>(host);
        }
    }
}
