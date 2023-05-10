using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using VKTest.Models;
using Microsoft.AspNetCore.Mvc;
using VKTest.Data;
using System.Text;

namespace VKTest.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration; 
        private readonly ApplicationContext _context;

        public TokenController(IConfiguration config)
        {
            _configuration = config;
            _context = new ApplicationContext();
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserInfo _userInfo)
        {
            if (_userInfo != null && _userInfo.Login != null && _userInfo.Password != null)
            {
                var user = await GetUser(_userInfo.Login, _userInfo.Password);
                if (user != null)
                {
                    var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("Login", user.Login),
                        new Claim("Password", user.Password),
                        new Claim("CreatedDate", user.CreatedDate.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }


        private async Task<User> GetUser(string login, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
        }
    }
}
