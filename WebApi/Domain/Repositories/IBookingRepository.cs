using WebApi.Domain.Dtos;

namespace WebApi.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking_DTO>> GetAllAsync(int page, int pageSize);
        Task<Booking_DTO> GetByIdAsync(Guid id);
        Task AddAsync(Booking_DTO booking);
        Task Update(Booking_DTO booking);
        Task Delete(Booking_DTO booking);
    }
}