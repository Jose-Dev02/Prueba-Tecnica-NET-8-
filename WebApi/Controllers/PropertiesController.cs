using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Dtos;
using WebApi.Domain.Repositories;
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
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var properties = await _propertyRepository.GetAllAsync(page, pageSize);
                return Ok(properties);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Property_DTO propertyRequest)
        {
            try
            {

                var existingHost = await _hostRepository.GetByIdAsync(propertyRequest.HostId);

                if (existingHost == null)
                {
                    return BadRequest(new { message = "Host not found." });
                }

                var propertyDto = new Property_DTO
                {
                    Id = Guid.NewGuid(),
                    Name = propertyRequest.Name,
                    Location = propertyRequest.Location,
                    HostId = propertyRequest.HostId,
                    PricePerNight = propertyRequest.PricePerNight,
                    Status = propertyRequest.Status
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
        public async Task<IActionResult> Update([FromBody] Property_DTO propertyRequest)
        {
            try
            {
                var existingProperty = await _propertyRepository.GetByIdAsync(propertyRequest.Id);

                if (existingProperty == null)
                {
                    return NotFound(new { message = "Property not exists."});
                }

                var existingHost = await _hostRepository.GetByIdAsync(propertyRequest.HostId);

                if(existingHost == null)
                {
                    return NotFound(new { message = "Host not exists." });
                }

                await _propertyRepository.Update(propertyRequest);
                await _unitOfWork.CompleteAsync();

                return Ok(propertyRequest);
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

                await _propertyRepository.Delete(property);
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