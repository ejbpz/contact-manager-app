using ContactsManager.Models;

namespace ContactsManager.RepositoryContracts
{
    /// <summary>
    /// Represents the data access logic to manage Country model.
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add a new country object to data store.
        /// </summary>
        /// <param name="country">Country objecto to add.</param>
        /// <returns>Returns the country object after been added into the data store.</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all the countries in the data store.
        /// </summary>
        /// <returns>All the countries in the table.</returns>
        Task<List<Country>> GetCountries();

        /// <summary>
        /// Retrieves the country based on the country ID.
        /// </summary>
        /// <param name="countryId">GUID referes to the country ID.</param>
        /// <returns>Returns a country or null.</returns>
        Task<Country?> GetCountryByCountryId(Guid countryId);

        /// <summary>
        /// Retrieves the country based on the country name.
        /// </summary>
        /// <param name="countryName">string of the country name.</param>
        /// <returns>Returns a country or null.</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}