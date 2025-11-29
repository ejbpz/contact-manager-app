using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Core.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "The field doesn't look like email")]
        [Remote(action: "IsEmailRegistered", controller: "Account", ErrorMessage = "Email is already in use")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password cannot be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Name cannot be blank")]
        public string PersonName { get; set; } = "";

        [Required(ErrorMessage = "Phone cannot be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone should containe only numbers")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Confirm password cannot be blank")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Should match with the password")]
        public string ConfirmPassword { get; set; } = "";

        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
    }
}
