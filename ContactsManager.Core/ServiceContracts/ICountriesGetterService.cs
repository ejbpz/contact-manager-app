using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Represent Business Logic for Country Entity manipulation.
    /// </summary>
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Retrieve all the countries in the list.
        /// </summary>
        /// <returns>Returns all the countries as a List of CountryResponse.</returns>
        Task<List<CountryResponse>> GetCountries();

        /// <summary>
        /// Finds a Country base on its country ID.
        /// </summary>
        /// <param name="countryId">GUID to be searched.</param>
        /// <returns>Returns the country with this GUID.</returns>
        Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);
    }
}
