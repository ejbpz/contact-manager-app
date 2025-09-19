using ContactsManager.Models;
using ContactsManager.Repository;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CountriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Country> AddCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetCountries()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryId(Guid countryId)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);
        }
    }
}
