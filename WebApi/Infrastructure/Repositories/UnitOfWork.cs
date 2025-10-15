using System.Threading.Tasks;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IPropertyRepository PropertyRepository { get; }

        public UnitOfWork(AppDbContext context, IPropertyRepository propertyRepository)
        {
            _context = context;
            PropertyRepository = propertyRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}