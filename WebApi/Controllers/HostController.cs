using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Dtos;
using WebApi.Domain.Repositories;
using Host = WebApi.Domain.Entities.Host;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HostsController(IHostRepository hostRepository, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IHostRepository _hostRepository = hostRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var hosts = await _hostRepository.GetAllAsync(page, pageSize);
                return Ok(hosts);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Host_DTO hostRequest)
        {
            try
            {
                var existingHost = await _hostRepository.GetByFullNameAsync(hostRequest.FullName,Guid.Empty);

                if (existingHost != null)
                {
                    return BadRequest(new { message = "Host with the same full name already exists." });
                }

                var newHost = new Host
                {
                    Id = Guid.NewGuid(),
                    FullName = hostRequest.FullName,
                    Email = hostRequest.Email,
                    Phone = hostRequest.Phone,
                    Properties = []
                };

                var createdHost = await _hostRepository.AddAsync(newHost);
                await _unitOfWork.CompleteAsync();
                return CreatedAtAction(nameof(GetAll), new { id = createdHost.Id }, createdHost);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500 );
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] Host_DTO hostRequest)
        {
            try
            {
                var existingHost = await _hostRepository.GetByIdAsync(hostRequest.Id);
                    
                if (existingHost == null)
                {
                    return NotFound(new { message = "Host not found" });
                }

                var hostWithSameFullName = await _hostRepository.GetByFullNameAsync(hostRequest.FullName, hostRequest.Id);
                if (hostWithSameFullName != null)
                {
                    return BadRequest(new { message = "Another host with the same full name already exists." });
                }

                await _hostRepository.UpdateAsync(hostRequest);
                await _unitOfWork.CompleteAsync();

                return Ok(hostRequest);
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
                var host = await _hostRepository.GetByIdAsync(id);

                if (host == null)
                {
                    return NotFound();
                }

                await _hostRepository.DeleteAsync(host);
                await _unitOfWork.CompleteAsync();

                return Ok(host);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}
