using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class CountriesService : ICountriesService
    {
        private PeopleDbContext _peopleDbContext;

        public CountriesService(PeopleDbContext peopleDbContext)
        {
            _peopleDbContext = peopleDbContext;
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest is null) throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName is null) throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if(_peopleDbContext.Countries.Where(country => country.CountryName == countryAddRequest.CountryName).Count() > 0) throw new ArgumentException("The country name given is already in the list.");

            Country newCountry = countryAddRequest.ToCountry();

            newCountry.CountryId = Guid.NewGuid();
            _peopleDbContext.Countries.Add(newCountry);
            _peopleDbContext.SaveChanges();

            return newCountry.ToCountryResponse();
        }

        public List<CountryResponse> GetCountries()
        {
            return _peopleDbContext.Countries.ToList().Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null) return null;

            return _peopleDbContext.Countries.FirstOrDefault(country => country.CountryId.Equals(countryId))?.ToCountryResponse();
        }
    }
}
