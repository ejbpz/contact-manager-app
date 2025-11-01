using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Business Logic for Person model manipulation.
    /// </summary>
    public interface IPeopleGetterService
    {
        /// <summary>
        /// Retrieve all the people in the list.
        /// </summary>
        /// <returns>Returns a list of people.</returns>
        Task<List<PersonResponse>> GetPeople();

        /// <summary>
        /// Request a person by its ID.
        /// </summary>
        /// <param name="personId">person's ID</param>
        /// <returns>Returns a person, base on its ID.</returns>
        Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

        /// <summary>
        /// Finds all the people that matched with the given filter and search string.
        /// </summary>
        /// <param name="searchBy">Filter to seach people.</param>
        /// <param name="query">Text to search.</param>
        /// <returns>Returns a list of people that match with the query.</returns>
        Task<List<PersonResponse>?> GetFilteredPeople(string searchBy, string? query);

        /// <summary>
        /// Returns people as CSV.
        /// </summary>
        /// <returns>Returns the memory stream with CSV data.</returns>
        Task<MemoryStream> GetPeopleCSV();

        /// <summary>
        /// Returns people as Excel.
        /// </summary>
        /// <returns>Returns the memory stream with Excel data.</returns>
        Task<MemoryStream> GetPeopleExcel();
    }
}
