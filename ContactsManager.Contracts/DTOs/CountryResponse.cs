using ContactsManager.Models;

namespace ContactsManager.ServiceContracts.DTOs
{
    /// <summary>
    /// DTO Class used to return type for almost every CountriesService methods.
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName,
            };
        }
    }
}
