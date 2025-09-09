using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Represent Business Logic for Country Entity manipulation.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Add a new country to the list of countries.
        /// </summary>
        /// <param name="countryAddRequest">Country object to add.</param>
        /// <returns>Returns the country object after been added.</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

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
