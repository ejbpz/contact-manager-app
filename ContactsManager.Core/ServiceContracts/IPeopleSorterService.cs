using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Business Logic for Person model manipulation.
    /// </summary>
    public interface IPeopleSorterService
    {
        /// <summary>
        /// Sort a group of people by user preferences.
        /// </summary>
        /// <param name="allPeople">Group of people to sort.</param>
        /// <param name="sortBy">Column or property.</param>
        /// <param name="sortOrder">Ascending or descending.</param>
        /// <returns>Returns a sorted group of people based on the property and the order selected.</returns>
        List<PersonResponse> GetSortedPeople(List<PersonResponse> allPeople, string sortBy, SortOrderOptions sortOrder);
    }
}
