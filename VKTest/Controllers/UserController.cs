using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VKTest.Data;
using VKTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Cache;
using System.Reflection;
using System.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VKTest.Controllers
{

    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationContext _context;
        private IMemoryCache _cache;

        public UserController(ILogger<UserController> logger, ApplicationContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
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

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(20))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);

            
            
            
            //List<string> a;
            //try
            //{
            //    
            //}
            //catch
            //{
            //    List<string> a = new List<string>();
            //}

            ////List<string> a = new List<string>();
            //a.Add("привет");
            //a.Add("привет1");
            //_cache.Set("User.Login", a, cacheEntryOptions);

            //try
            //{
            //    _cache.TryGetValue("User.Login", out List<string> b);
            //    foreach (string i in b)
            //    {
            //        _logger.LogInformation($"{i}");
            //    }


            //}
            //catch
            //{
            //    _logger.LogInformation("’уйн€");
            //}

            

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

                // попытка сделать запрет регистрации пользователей при отправлени€ запросов с одинаковыми login с интервалом меньше 5 секунд
                // сначала пыталс€ использовать сессии, потом узнал, что они доступны, только внутри одного клиента
                // потом узнал про кэш, но к сожалению не нашел как получать все доступные ключи из него 
                // пыталс€ сделать свой список логинов, которые хран€тс€ по 5 секунд во врем€ регистрации и удал€ютс€ из этого списка после этого срока
                // но почему-то не кэш был пуст по этому ключу посто€нно
                // надеюсь будет разбор, как грамотно это сделать
                /*
                int numberOfDelaysOf250MilliSeconds = 20; 
                _cache.TryGetValue("User.Login", out List<string> loginsOfTemporacyRegistation);
                _logger.LogInformation($"{loginsOfTemporacyRegistation.Count}");
                if (loginsOfTemporacyRegistation != null && loginsOfTemporacyRegistation.Count != 0)
                {
                    loginsOfTemporacyRegistation.Add(userDTO.Login);
                    _cache.Set("User.Login", loginsOfTemporacyRegistation, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("User.Login", new List<string>(), cacheEntryOptions);
                }
                bool error = false;
                for (int i = 0; i < numberOfDelaysOf250MilliSeconds; i++)
                {
                    Task task = Task.Run(async () =>
                    {
                        _cache.TryGetValue("User.Login", out List<string> loginsOfTemporacyRegistation);
                        if (loginsOfTemporacyRegistation != null && loginsOfTemporacyRegistation.Count != 0)
                        {
                            loginsOfTemporacyRegistation = new List<string>();
                        }
                        if (!(loginsOfTemporacyRegistation.Where(x => x == userDTO.Login).Count() < 2))
                        {
                            loginsOfTemporacyRegistation.Remove(userDTO.Login);
                            _cache.Set("User.Login", loginsOfTemporacyRegistation, cacheEntryOptions);
                            error = true;
                        }
                        await Task.Delay(250);

                    });
                    task.GetAwaiter().GetResult();
                    task.Wait();
                    if (error)
                    {
                        return StatusCode(403, "There was an attempt to register a user with the same login just now, registration was denied");
                    }
                }
                if (loginsOfTemporacyRegistation != null && loginsOfTemporacyRegistation.Count != 0)
                {
                    loginsOfTemporacyRegistation.Remove(userDTO.Login);
                    _cache.Set("User.Login", loginsOfTemporacyRegistation, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("User.Login", new List<string>(), cacheEntryOptions);
                }
                */
                

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