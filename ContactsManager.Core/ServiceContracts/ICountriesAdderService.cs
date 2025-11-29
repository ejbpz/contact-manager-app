using Microsoft.AspNetCore.Http;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.ServiceContracts
{
    /// <summary>
    /// Represent Business Logic for Country Entity manipulation.
    /// </summary>
    public interface ICountriesAdderService
    {
        /// <summary>
        /// Add a new country to the list of countries.
        /// </summary>
        /// <param name="countryAddRequest">Country object to add.</param>
        /// <returns>Returns the country object after been added.</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);


        /// <summary>
        /// Uploads countries from Excel file into database.
        /// </summary>
        /// <param name="formFile">Excel file with list of countries.</param>
        /// <returns>Returns the number of countries added.</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
