using System.ComponentModel.DataAnnotations;

namespace MVC_CabServices.Models
{
    public class UserRegister
    {
        public string? EmailId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; } = null!;
        public int? UserRoleId { get; set; }
        public string? MobileNumber { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
