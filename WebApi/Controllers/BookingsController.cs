using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Dtos;
using WebApi.Domain.Repositories;
using WebApi.Domain.RequestObjects;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController(AppDbContext context, IBookingRepository bookingRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IPropertyRepository _propertyRepository = propertyRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var bookings = await _bookingRepository.GetAllAsync(page, pageSize);
                return Ok(bookings);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BookingRequest bookingRequest)
        {
            try
            {
                var property = await _propertyRepository.GetByIdAsync(bookingRequest.PropertyId);

                if (property == null)
                {
                    return BadRequest(new { message = "Property not found." });
                }

                var bookingDto = new Booking_DTO
                {
                    Id = Guid.NewGuid(),
                    PropertyId = bookingRequest.PropertyId,
                    CheckIn = bookingRequest.CheckIn,
                    CheckOut = bookingRequest.CheckOut,
                    TotalPrice = bookingRequest.TotalPrice
                };

                await _bookingRepository.AddAsync(bookingDto);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetAll), new { id = bookingDto.Id }, bookingDto);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] BookingRequest bookingRequest)
        {
            try
            {
                var existingBooking = await _bookingRepository.GetByIdAsync(id);

                if (existingBooking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }

                var property = await _propertyRepository.GetByIdAsync(bookingRequest.PropertyId);

                if (property == null)
                {
                    return BadRequest(new { message = "Property not found." });
                }

                existingBooking.CheckIn = bookingRequest.CheckIn;
                existingBooking.CheckOut = bookingRequest.CheckOut;
                existingBooking.TotalPrice = bookingRequest.TotalPrice;
                existingBooking.PropertyId = bookingRequest.PropertyId;

                await _bookingRepository.Update(existingBooking);
                await _unitOfWork.CompleteAsync();

                return Ok(existingBooking);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);

                if (booking == null)
                {
                    return NotFound();
                }

                await _bookingRepository.Delete(booking);
                await _unitOfWork.CompleteAsync();

                return Ok(booking);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}