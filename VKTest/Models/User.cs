using System.ComponentModel.DataAnnotations;

namespace VKTest.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]   
        public string Login { get; set; }
        
        [Required]
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int UserGroupId { get; set; }
        public int UserStateId { get; set; }

    }
}
