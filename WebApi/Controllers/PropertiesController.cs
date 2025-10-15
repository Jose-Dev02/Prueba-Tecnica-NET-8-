using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PropertiesController(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
        {
            _propertyRepository = propertyRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null, [FromQuery] string? address = null)
        {
            Expression<Func<Property, bool>>? filter = null;

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(address))
            {
                filter = p => (name == null || p.Name.Contains(name)) && (address == null || p.Address.Contains(address));
            }

            var properties = await _propertyRepository.GetAllAsync(page, pageSize, filter);
            return Ok(properties);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Property property)
        {
            await _propertyRepository.AddAsync(property);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(GetAll), new { id = property.Id }, property);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] Property property)
        {
            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            existingProperty.Name = property.Name;
            existingProperty.Address = property.Address;
            existingProperty.HostId = property.HostId;

            _propertyRepository.Update(existingProperty);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            _propertyRepository.Delete(property);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpPost("sync")]
        [Authorize]
        public async Task<IActionResult> Sync()
        {
            var domainEvent = new DomainEvent
            {
                EventType = "Sync",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}