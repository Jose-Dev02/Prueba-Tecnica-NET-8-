using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Repositories;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTAsSyncController(AppDbContext context, IPropertyRepository propertyRepository,IOTAsRepository otasRepository, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPropertyRepository _propertyRepository = propertyRepository;
        private readonly IOTAsRepository _otaRepository = otasRepository;

        [HttpGet("sync")]
        [Authorize]
        public async Task<IActionResult> Sync([FromQuery] Guid id)
        {
            try
            {
                var property = await _propertyRepository.GetByIdAsync(id);
                if (property == null)
                {
                    return BadRequest(new { message = "No property found to associate with the event." });
                }

                var domainEvent = await _otaRepository.Sync(id);

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
