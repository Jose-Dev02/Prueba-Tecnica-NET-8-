using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApi.Domain.Dtos;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;
using WebApi.Domain.Entities;
using AutoMapper;
using System.Text.Json;

namespace WebApi.Infrastructure.Repositories
{
    public class BookingRepository(AppDbContext context, IMapper mapper) : IBookingRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<Booking_DTO>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Bookings
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => _mapper.Map<Booking_DTO>(b))
                .ToListAsync();
        }

        public async Task<Booking_DTO> GetByIdAsync(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            return _mapper.Map<Booking_DTO>(booking);
        }

        public async Task AddAsync(Booking_DTO bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var overlappingBooking = await _context.Bookings
                    .AnyAsync(b => b.PropertyId == bookingDto.PropertyId &&
                                   b.CheckOut > bookingDto.CheckIn &&
                                   b.CheckIn < bookingDto.CheckOut);

                if (overlappingBooking)
                {
                    throw new InvalidOperationException("The property is already booked for the selected dates.");
                }

                var booking = _mapper.Map<Bookings>(bookingDto);
                await _context.Bookings.AddAsync(booking);

                var property = await _context.Properties.FindAsync(bookingDto.PropertyId) 
                               ?? throw new InvalidOperationException("Property not found.");

                var registerEvent = new DomainEvent
                {
                    PropertyId = bookingDto.PropertyId,
                    EventType = "BookingCreated",
                    PayloadJSON = JsonSerializer.Serialize(new 
                    { 
                        bookingId = booking.Id, 
                        checkIn = booking.CheckIn, 
                        checkOut = booking.CheckOut,
                        totalPrice = booking.TotalPrice,
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

        public async Task Update(Booking_DTO bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var overlappingBooking = await _context.Bookings
                    .AnyAsync(b => b.PropertyId == bookingDto.PropertyId &&
                                   b.Id != bookingDto.Id &&
                                   b.CheckOut > bookingDto.CheckIn &&
                                   b.CheckIn < bookingDto.CheckOut);

                if (overlappingBooking)
                {
                    throw new InvalidOperationException("The property is already booked for the selected dates.");
                }

                var booking = _context.Bookings.Find(bookingDto.Id);
                if (booking != null)
                {
                    _mapper.Map(bookingDto, booking);
                    _context.Bookings.Update(booking);

                    var property = await _context.Properties.FindAsync(booking.PropertyId) ?? throw new InvalidOperationException("Property not found.");
                    var registerEvent = new DomainEvent
                    {
                        PropertyId = booking.PropertyId,
                        EventType = "BookingUpdated",
                        PayloadJSON = JsonSerializer.Serialize(new
                        {
                            bookingId = booking.Id,
                            checkIn = booking.CheckIn,
                            checkOut = booking.CheckOut,
                            totalPrice = booking.TotalPrice,
                        }),
                        Property = property,
                    };

                    await _context.DomainEvents.AddAsync(registerEvent);
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

        public async Task Delete(Booking_DTO bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = _context.Bookings.Find(bookingDto.Id);
                if (booking != null)
                {
                    _context.Bookings.Remove(booking);

                    var property = await _context.Properties.FindAsync(booking.PropertyId) ?? throw new InvalidOperationException("Property not found.");
                    var registerEvent = new DomainEvent
                    {
                        PropertyId = booking.PropertyId,
                        EventType = "BookingDeleted",
                        PayloadJSON = JsonSerializer.Serialize(new
                        {
                            bookingId = booking.Id,
                            checkIn = booking.CheckIn,
                            checkOut = booking.CheckOut,
                            totalPrice = booking.TotalPrice,

                        }),
                        Property = property,
                    };

                    await _context.DomainEvents.AddAsync(registerEvent);
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