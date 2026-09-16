using Microsoft.AspNetCore.Mvc;
using SimpleJwtImplementation.Infrastructrure;
using SimpleJwtImplementation.Models;
using SimpleJwtImplementation.Models.Dtos;

namespace SimpleJwtImplementation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IList<User> users, ITokenService tokenService) : ControllerBase
    {
        private readonly IList<User> _users = users;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            User? user = _users.SingleOrDefault(u => u.Email == dto.Email);

            if (user is null)
                return NotFound();

            string token = _tokenService.GenerateToken(user);

            return Ok(new UserDto(user.Id, user.Nom, user.Prenom, user.Email, user.Role, token));
        }
    }
}
