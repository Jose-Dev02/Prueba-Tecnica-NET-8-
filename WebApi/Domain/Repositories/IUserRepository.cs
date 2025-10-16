using WebApi.Domain.Entities;

namespace WebApi.Domain.Repositories
{
    public interface IUserRepository
    {
        public Task AddAsync(User property);
    }
}
