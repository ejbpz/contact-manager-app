using ContactsManager.Models;
using ContactsManager.ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.ServiceContracts.DTOs
{
    /// <summary>
    /// DTO to insert a new person.
    /// </summary>
    public class PersonAddRequest
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Person Name cannot be null.")]
        public string? PersonName { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Email cannot be null.")]
        [RegularExpression("^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$", ErrorMessage = "This text doesn't match to an email.")]
        public string? PersonEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public GenderOptions? Gender { get; set; }

        [DataType(DataType.Text)]
        public Guid? CountryId { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        public bool IsReceivingNewsLetters { get; set; }

        /// <summary>
        /// Converts the current object to a Person type.
        /// </summary>
        /// <returns>Returns a Person object.</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonName = PersonName,
                PersonEmail = PersonEmail,  
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address,
                IsReceivingNewsLetters = IsReceivingNewsLetters,
            };
        }
    }
}
