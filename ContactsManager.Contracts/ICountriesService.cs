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
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest);
    }
}
