using ContactsManager.Models;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.ServiceContracts.DTOs
{
    /// <summary>
    /// DTO to insert a new person.
    /// </summary>
    public class PersonAddRequest
    {
        public string? PersonName { get; set; }
        public string? PersonEmail { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
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
