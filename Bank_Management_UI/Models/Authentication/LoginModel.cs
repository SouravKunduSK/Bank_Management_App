using System.ComponentModel.DataAnnotations;

namespace Bank_Management_UI.Models.Authentication
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Email is required!")]
        [DataType(DataType.EmailAddress, ErrorMessage ="Please, provide valid email address!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should contain atleast 6 characters.")]
        [RegularExpression(@"^(?=.*[^A-Za-z0-9]).*(?=.*[a-z]).*(?=.*[A-Z]).*(?=.*\d).*$", ErrorMessage = "Invalid input! The string must contain at least one special character, one lowercase letter, one uppercase letter, and one digit.")]

        public string Password { get; set; }
        
        public bool RememberMe { get; set; } = false;
    }
}
