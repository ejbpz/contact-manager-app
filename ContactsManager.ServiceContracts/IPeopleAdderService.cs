using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Business Logic for Person model manipulation.
    /// </summary>
    public interface IPeopleAdderService
    {
        /// <summary>
        /// Add a new Person in the People list.
        /// </summary>
        /// <param name="personAddRequest">Person to be added</param>
        /// <returns>Returns the same Person with an ID.</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
