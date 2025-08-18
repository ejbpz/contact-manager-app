using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;

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

        /// <summary>
        /// Finds all the people that matched with the given filter and search string.
        /// </summary>
        /// <param name="searchBy">Filter to seach people.</param>
        /// <param name="query">Text to search.</param>
        /// <returns>Returns a list of people that match with the query.</returns>
        List<PersonResponse> GetFilteredPeople(string searchBy, string? query);

        /// <summary>
        /// Sort a group of people by user preferences.
        /// </summary>
        /// <param name="allPeople">Group of people to sort.</param>
        /// <param name="sortBy">Column or property.</param>
        /// <param name="sortOrder">Ascending or descending.</param>
        /// <returns>Returns a sorted group of people based on the property and the order selected.</returns>
        List<PersonResponse> GetSortedPeople(List<PersonResponse> allPeople, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates an specific person details by its id.
        /// </summary>
        /// <param name="personUpdateRequest">Person to be updated.</param>
        /// <returns>Returns the same person with its attributes updated.</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Deletes a person using its id.
        /// </summary>
        /// <param name="personId">Id to the person to be deleted.</param>
        /// <returns>Returns bool to know if was deleted.</returns>
        bool DeletePerson(Guid? personId);
    }
}
