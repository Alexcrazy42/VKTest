using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VKTest.Data;
using VKTest.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace VKTest.Controllers
{

    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UserController()
        {
            _context = new ApplicationContext();
        }



        
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllUsers() // 
        {
            if (_context.Users == null)
            {
                return NotFound();
            }


            var users = from u in _context.Users
                        join ug in _context.UserGroups on u.UserGroupId equals ug.Id into ug
                        from uug1 in ug.DefaultIfEmpty()
                        join us in _context.UserStates on u.UserStateId equals us.Id into ugs
                        from ugs1 in ugs.DefaultIfEmpty()
                        select new
                        {
                            Id = u.Id,
                            Login = u.Login,
                            Password = u.Password,
                            CreatedDate = u.CreatedDate,
                            UserGroup = uug1.Code,
                            UserState = ugs1.Code,
                        };


            

            return await users.ToListAsync();

        }


        [HttpGet]
        public async Task<ActionResult<object>> GetSomeUsers(int firstId, int lastId)
        {

            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = from u in _context.Users
                        where u.Id >= firstId && u.Id <= lastId
                        join ug in _context.UserGroups on u.UserGroupId equals ug.Id into ug
                        from uug1 in ug.DefaultIfEmpty()
                        join us in _context.UserStates on u.UserStateId equals us.Id into ugs
                        from ugs1 in ugs.DefaultIfEmpty()
                        select new
                        {
                            Id = u.Id,
                            Login = u.Login,
                            Password = u.Password,
                            CreatedDate = u.CreatedDate,
                            UserGroup = uug1.Code,
                            UserState = ugs1.Code

                        };

            if (users.ToArray().Length == 0)
            {
                return NotFound();
            }

            return await users.ToListAsync();
            
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUserById(int id) 
        {

            if (_context.Users == null)
            {
                return NotFound();
            }

            var user = from u in _context.Users where u.Id == id
                        join ug in _context.UserGroups on u.UserGroupId equals ug.Id into ug
                        from uug1 in ug.DefaultIfEmpty()
                        join us in _context.UserStates on u.UserStateId equals us.Id into ugs
                        from ugs1 in ugs.DefaultIfEmpty()
                        select new
                        {
                            Id = u.Id,
                            Login = u.Login,
                            Password = u.Password,
                            CreatedDate = u.CreatedDate,
                            UserGroup = uug1.Code,
                            UserState = ugs1.Code

                        };

            

            if (user.ToArray().Length == 0)
            {
                return NotFound();
            }

            return user.ToListAsync().Result;
        }




        [AllowAnonymous]       
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
        {
            
            if (HasUserWithThisLogin(userDTO.Login))
            {
                return StatusCode(403, "This login is taken"); 
            }

            var userGroup = new UserGroup()
            {
                Code = userDTO.UserGroupCode,
                Description = ""
            };
            var userState = new UserState()
            {
                Code = "Active",
                Description = ""
            };

            if (userGroup.Code == "Admin" && HasAdmin())
            {
                return StatusCode(403, "Can't have more then one admin");
            }
            else
            {
                _context.UserStates.Add(userState);
                _context.UserGroups.Add(userGroup);
                await _context.SaveChangesAsync();

                var user = new User()
                {
                    Login = userDTO.Login,
                    Password = userDTO.Password,
                    CreatedDate = DateTime.UtcNow,
                    UserGroupId = userGroup.Id,
                    UserStateId = userState.Id
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();




                return CreatedAtAction(
                    nameof(GetUserById),
                    new
                    {
                        Id = user.Id,
                        Name = user.Login,
                        Password = user.Password,
                        CreatedDate = user.CreatedDate,
                        UserGroupId = user.UserGroupId,
                        UserStateId = user.UserStateId
                    },
                    userDTO.UserToDTO(user));
            }


            

            
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userStateToUpdate = _context.UserStates.Where(u => u.Id == user.UserStateId).Single();
            if (userStateToUpdate != null)
            {
                userStateToUpdate.Code = "Blocked";
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }


        private bool HasAdmin()
        {
            var admin = _context.UserGroups.Where(u => u.Code == "Admin");
            if (admin.ToArray().Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HasUserWithThisLogin(string login)
        {
            var user = _context.Users.Where(u => u.Login == login);
            if (user.ToArray().Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        



    }
}