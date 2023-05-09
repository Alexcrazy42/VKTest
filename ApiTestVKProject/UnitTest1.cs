using Microsoft.AspNetCore.Mvc;
using VKTest.Controllers;
using VKTest.Models;
using VKTest.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiTestVKProject
{
    public class UnitTest1
    {
        UserController userController = new UserController();
        Random random = new Random();

        [Fact]
        public async void TestForGetAllUsers()
        {
            
            var test = userController.GetAllUsers();
            Assert.Equal(typeof(Task<ActionResult<IEnumerable<object>>>), test.GetType());
        }

        [Fact]
        public async void TestForGetOneUser()
        {
            int id = random.Next();
            var test = userController.GetUserById(id);
            Assert.Equal(typeof(Task<ActionResult<object>>), test.GetType());
        }

        [Fact]
        public async void TestForGetSomeUsers()
        {
            
            int firstId = random.Next();
            int lastId = random.Next();
            var test = userController.GetSomeUsers(firstId, lastId);
            Assert.Equal(typeof(Task<ActionResult<IEnumerable<object>>>), test.GetType());
        }

        [Fact]
        public async void TestForPostUser()
        {
            UserDTO userDTO = new UserDTO();
            var test = userController.PostUser(userDTO);
            
            Assert.Equal(typeof(Task<ActionResult<UserDTO>>), test.GetType());
        }

        [Fact]
        public async void TestForDeleteUser()
        {
            var id = random.Next();
            var test = userController.DeleteUser(id);
            Assert.Equal(typeof(Task<IActionResult>), test.GetType());
        }
    }
}