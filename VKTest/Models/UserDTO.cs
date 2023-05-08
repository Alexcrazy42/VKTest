using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKTest.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string UserGroupCode { get; set; }

        public UserDTO UserToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
            };
        }
    }

    
}
