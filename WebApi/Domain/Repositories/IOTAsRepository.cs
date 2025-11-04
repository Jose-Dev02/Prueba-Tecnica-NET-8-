using WebApi.Domain.Dtos;

namespace WebApi.Domain.Repositories
{
    public interface IOTAsRepository
    {
        public Task<DomainEvent_DTO?> Sync(Guid id);
    }
}
