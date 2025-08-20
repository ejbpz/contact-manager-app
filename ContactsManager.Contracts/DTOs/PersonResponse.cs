using ContactsManager.Models;
using ContactsManager.ServiceContracts.Enums;
using System.Net;
using System.Reflection;

namespace ContactsManager.ServiceContracts.DTOs
{
    /// <summary>
    /// DTO class that is going to be used to return most of the Person Service methods.
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? PersonEmail { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public double? Age { get; set; }
        public bool IsReceivingNewsLetters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"PersonId: {PersonId}, " +
                $"PersonName: {PersonName}, " +
                $"PersonEmail: {PersonEmail}, " +
                $"DateOfBirth: {DateOfBirth?.ToString("dd MMMM yyyy")}, " +
                $"Gender: {Gender}, " +
                $"CountryId: {CountryId}, " +
                $"CountryName: {CountryName}, " +
                $"Address: {Address}, " +
                $"Age: {Age}, " +
                $"IsReceivingNewsLetters: {IsReceivingNewsLetters}";
        }

        /// <summary>
        /// Overriding Equals method to compare their attributes.
        /// </summary>
        /// <param name="obj">PersonResponse object to compare.</param>
        /// <returns>Returns true or false, depends of the matched.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse personResponse = (PersonResponse)obj;
            return this.PersonName == personResponse.PersonName &&
                this.PersonEmail == personResponse.PersonEmail &&
                this.DateOfBirth == personResponse.DateOfBirth &&
                this.Gender == personResponse.Gender &&
                this.CountryId == personResponse.CountryId &&
                this.CountryName == personResponse.CountryName &&
                this.Address == personResponse.Address &&
                this.IsReceivingNewsLetters == personResponse.IsReceivingNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                Address = Address,
                PersonId = PersonId,
                CountryId = CountryId,
                PersonName = PersonName,
                PersonEmail = PersonEmail,
                DateOfBirth = DateOfBirth,
                IsReceivingNewsLetters = IsReceivingNewsLetters,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender ?? "Other", true),
            };
        }
    }

    public static class PersonExtensions
    {
        /// <summary>
        /// Extension method to convert an object of Person into PersonResponse class.
        /// </summary>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                Gender = person.Gender,
                Address = person.Address,
                PersonId = person.PersonId,
                CountryId = person.CountryId,
                PersonName = person.PersonName,
                PersonEmail = person.PersonEmail,
                DateOfBirth = person.DateOfBirth,
                IsReceivingNewsLetters = person.IsReceivingNewsLetters,
                Age = (person.DateOfBirth is not null) 
                    ? Math.Floor((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) 
                    : null,
            };
        }


    }
}
