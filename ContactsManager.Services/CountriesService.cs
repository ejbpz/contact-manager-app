using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Services
{
    public class CountriesService : ICountriesService
    {
        private PeopleDbContext _peopleDbContext;

        public CountriesService(PeopleDbContext peopleDbContext)
        {
            _peopleDbContext = peopleDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest is null) throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName is null) throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if(await _peopleDbContext.Countries.Where(country => country.CountryName == countryAddRequest.CountryName).CountAsync() > 0) throw new ArgumentException("The country name given is already in the list.");

            Country newCountry = countryAddRequest.ToCountry();

            newCountry.CountryId = Guid.NewGuid();
            _peopleDbContext.Countries.Add(newCountry);
            await _peopleDbContext.SaveChangesAsync();

            return newCountry.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetCountries()
        {
            return await _peopleDbContext.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null) return null;

            Country? countryResponse = await _peopleDbContext.Countries.FirstOrDefaultAsync(country => country.CountryId.Equals(countryId));

            if (countryResponse is null) return null;

            return countryResponse.ToCountryResponse();
        }
    }
}
