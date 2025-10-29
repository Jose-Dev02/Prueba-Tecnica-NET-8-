using Microsoft.EntityFrameworkCore.Storage;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;
        public async Task AddAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
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
    }
}
