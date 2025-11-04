using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Domain.RequestObjects;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Repositories;

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

        [HttpPost("sync")]
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

                var domainEvent = _otaRepository.Sync(id);

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
