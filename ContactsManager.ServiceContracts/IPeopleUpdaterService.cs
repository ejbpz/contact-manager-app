using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Business Logic for Person model manipulation.
    /// </summary>
    public interface IPeopleUpdaterService
    {
        /// <summary>
        /// Updates an specific person details by its id.
        /// </summary>
        /// <param name="personUpdateRequest">Person to be updated.</param>
        /// <returns>Returns the same person with its attributes updated.</returns>
        Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
