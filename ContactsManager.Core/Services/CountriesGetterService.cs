using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        private ICountriesRepository _countriesRepository;

        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<List<CountryResponse>> GetCountries()
        {
            return (await _countriesRepository.GetCountries()).Select(c => c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null || !countryId.HasValue) return null;

            var countryResponse = await _countriesRepository.GetCountryByCountryId(countryId.Value);

            if (countryResponse is null) return null;

            return countryResponse.ToCountryResponse();
        }
    }
}
