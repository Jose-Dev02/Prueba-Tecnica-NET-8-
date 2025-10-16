using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApi.Domain.Dtos;
using WebApi.Domain.Repositories;
using WebApi.Domain.RequestObjects;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController(AppDbContext context, IPropertyRepository propertyRepository, IHostRepository hostRepository, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IPropertyRepository _propertyRepository = propertyRepository;
        private readonly IHostRepository _hostRepository = hostRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null, [FromQuery] string? address = null)
        {
            try
            {
                Expression<Func<Property_DTO, bool>>? filter = null;

                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(address))
                {
                    filter = p => (name == null || p.Name.Contains(name)) && (address == null || p.Address.Contains(address));
                }

                var properties = await _propertyRepository.GetAllAsync(page, pageSize, filter);
                return Ok(properties);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] PropertyRequest propertyRequest)
        {
            try
            {

                var existingHost = await _hostRepository.GetByIdAsync(propertyRequest.HostId);

                var existingProperty = await _propertyRepository.GetByNameAsync(propertyRequest.Name, propertyRequest.HostId);

                if (existingProperty != null)
                {
                    return BadRequest(new { message = "Property with the same name already exists." });
                }

                if (existingHost == null)
                {
                    return BadRequest(new { message = "Host not found." });
                }

                var propertyDto = new Property_DTO
                {
                    Id = Guid.NewGuid(),
                    Name = propertyRequest.Name,
                    Address = propertyRequest.Address,
                    HostId = propertyRequest.HostId
                };

                await _propertyRepository.AddAsync(propertyDto);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetAll), new { id = propertyDto.Id }, propertyDto);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] PropertyRequest propertyRequest)
        {
            try
            {
                var existingProperty = await _propertyRepository.GetByIdAsync(id);

                if (existingProperty == null)
                {
                    return NotFound(new { message = "Property not exists."});
                }

                var existingHost = await _hostRepository.GetByIdAsync(propertyRequest.HostId);

                if(existingHost == null)
                {
                    return NotFound(new { message = "Host not exists." });
                }

                var validatingNewObject = await _propertyRepository.GetByNameAsync(propertyRequest.Name, propertyRequest.HostId);

                if(validatingNewObject != null)
                {
                    return BadRequest(new { message = "Another property with the same name already exists." });
                }

                existingProperty.Name = propertyRequest.Name;
                existingProperty.Address = propertyRequest.Address;
                existingProperty.HostId = propertyRequest.HostId;

                _propertyRepository.Update(existingProperty);
                await _unitOfWork.CompleteAsync();

                return Ok(existingProperty);
            }
            catch(Exception e)
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
                var property = await _propertyRepository.GetByIdAsync(id);

                if (property == null)
                {
                    return NotFound();
                }

                _propertyRepository.Delete(property);
                await _unitOfWork.CompleteAsync();

                return Ok(property);
            }
            catch(Exception e)
            {
                return Problem( detail: e.Message, statusCode: 500);
            }
        }
    }
}