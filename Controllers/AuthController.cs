using DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace infomatica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly InfomaticaContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(InfomaticaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username && u.Password == login.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            //var token = GenerateJwtToken(user);
            return Ok(user);
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Logout logic can be handled on the client-side by deleting the token.
            return Ok("Logged out");
        }

        //private string GenerateJwtToken(User user)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(30),
        //        signingCredentials: creds);
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
