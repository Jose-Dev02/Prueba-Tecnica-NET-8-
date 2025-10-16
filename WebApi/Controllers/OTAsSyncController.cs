using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var domainEvent = new DomainEvent
                {
                    EventType = "Sync",
                    CreatedAt = DateTime.UtcNow
                };

                _context.DomainEvents.Add(domainEvent);
                await _unitOfWork.CompleteAsync();
                return Ok(new { message = "OTAs synchronized successfully" });
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}
