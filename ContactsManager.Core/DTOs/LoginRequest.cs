using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "The input should be an email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password cannot be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
