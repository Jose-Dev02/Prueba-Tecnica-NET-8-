using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Domain.Entities;
using WebApi.Domain.Repositories;
using WebApi.Domain.RequestObjects;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Repositories;
using WebApi.Infrastructure.Utilities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AppDbContext context, IConfiguration configuration, IUnitOfWork unitOfWork ,IUserRepository userRepository) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IUserRepository _userRepository = userRepository;

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserRequest loginRequest)
        {
            // Generar el hash de la contraseña ingresada
            var hashedPassword = HashPassword256.HashPassword(loginRequest.Password);

            // Buscar el usuario en la base de datos
            var user = _context.Users.FirstOrDefault(u => u.Username == loginRequest.Username && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        }),
                Expires = DateTime.UtcNow.AddMonths(1),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromBody] UserRequest signRequest)
        {
            try
            {

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == signRequest.Username);

                if (user != null)
                {
                    return BadRequest("Username Exist");
                }

                var hashedPassword = HashPassword256.HashPassword(signRequest.Password);

                var newUser = new User
                {
                    Username = signRequest.Username,
                    PasswordHash = hashedPassword
                };

                await _userRepository.AddAsync(newUser);
                await _unitOfWork.CompleteAsync();



                return Ok(new
                {
                    newUser.Id,
                    newUser.Username
                });
            }
            catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}