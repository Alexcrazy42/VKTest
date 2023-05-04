using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VKTest.Data;
using VKTest.Models;

namespace VKTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _context;

        

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "User")]
        public  async Task<ActionResult<IEnumerable<User>>> Get()
        {

            return await _context.Users
                .ToArrayAsync();
        }
    }
}