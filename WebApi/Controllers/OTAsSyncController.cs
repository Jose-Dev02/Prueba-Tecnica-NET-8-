using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTAsSyncController(AppDbContext context, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpPost("sync")]
        [Authorize]
        public async Task<IActionResult> Sync()
        {
            try
            {
                var property = await _context.Properties.FirstOrDefaultAsync();
                if (property == null)
                {
                    return BadRequest(new { message = "No property found to associate with the event." });
                }

                var domainEvent = new DomainEvent
                {
                    EventType = "SyncWithOTAs",
                    CreatedAt = DateTime.UtcNow,
                    Property = property,
                    PayloadJSON = JsonSerializer.Serialize(new
                    {
                        ota = "Booking.com", 
                        status = "completed"

                    }),
                };

                _context.DomainEvents.Add(domainEvent);
                await _unitOfWork.CompleteAsync();
                return Ok(domainEvent);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}
