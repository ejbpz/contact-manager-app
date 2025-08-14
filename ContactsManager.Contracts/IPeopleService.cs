using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Business Logic for Person model manipulation.
    /// </summary>
    public interface IPeopleService
    {
        /// <summary>
        /// Add a new Person in the People list.
        /// </summary>
        /// <param name="personAddRequest">Person to be added</param>
        /// <returns>Returns the same Person with an ID.</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Retrieve all the people in the list.
        /// </summary>
        /// <returns>Returns a list of people.</returns>
        List<PersonResponse> GetPeople();

        /// <summary>
        /// Request a person by its ID.
        /// </summary>
        /// <param name="personId">person's ID</param>
        /// <returns>Returns a person, base on its ID.</returns>
        PersonResponse? GetPersonByPersonId(Guid? personId);
    }
}
