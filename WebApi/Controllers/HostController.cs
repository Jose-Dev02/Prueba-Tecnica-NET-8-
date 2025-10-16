using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Domain.RequestObjects;
using WebApi.Infrastructure.Persistence;
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
        public async Task<IActionResult> Create([FromBody] HostRequest hostRequest)
        {
            try
            {
                var existingHost = await _hostRepository.GetByNameAsync(hostRequest.Name);

                if (existingHost != null)
                {
                    return BadRequest(new { message = "Host with the same name already exists." });
                }

                var newHost = new Host
                {
                    Id = Guid.NewGuid(),
                    Name = hostRequest.Name,
                    Properties = []
                };

                await _hostRepository.AddAsync(newHost);
                await _unitOfWork.CompleteAsync();
                return CreatedAtAction(nameof(GetAll), new { id = newHost.Id }, newHost);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromQuery] Guid? id, [FromQuery] string? name, [FromBody] HostRequest hostRequest)
        {
            try
            {
                if (id == null && string.IsNullOrEmpty(name))
                {
                    return BadRequest(new { message = "You must provide either an id or a name to perform the update." });
                }

                var existingHost = id != null
                    ? await _hostRepository.GetByIdAsync(id)
                    : await _hostRepository.GetByNameAsync(name);

                if (existingHost == null)
                {
                    return NotFound(new { message = "Host not found" });
                }

                var hostWithSameName = await _hostRepository.GetByNameAsync(hostRequest.Name);
                if (hostWithSameName != null)
                {
                    return BadRequest(new { message = "Another host with the same name already exists." });
                }

                existingHost.Name = hostRequest.Name;

                _hostRepository.Update(existingHost);
                await _unitOfWork.CompleteAsync();

                return Ok(existingHost);
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery] Guid? id, [FromQuery] string? name)
        {
            try
            {
                if (id == null && string.IsNullOrEmpty(name))
                {
                    return BadRequest(new { message = "You must provide either an id or a name to perform the deletion." });
                }

                var host = id != null
                    ? await _hostRepository.GetByIdAsync(id.Value)
                    : await _hostRepository.GetByNameAsync(name);

                if (host == null)
                {
                    return NotFound();
                }

                _hostRepository.Delete(host);
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
