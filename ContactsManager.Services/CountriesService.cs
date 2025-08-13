using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using System.Diagnostics.Metrics;

namespace ContactsManager.Services
{
    public class CountriesService : ICountriesService
    {
        private List<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest is null) throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName is null) throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if(_countries.Where(country => country.CountryName == countryAddRequest.CountryName).Count() > 0) throw new ArgumentException("The country name given is already in the list.");

            Country newCountry = countryAddRequest.ToCountry();

            newCountry.CountryId = Guid.NewGuid();
            _countries.Add(newCountry);

            return newCountry.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null) return null;

            return _countries.FirstOrDefault(country => country.CountryId == countryId)?.ToCountryResponse();
        }
    }
}
