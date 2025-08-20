using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class CountriesService : ICountriesService
    {
        private List<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>()
                {
                    new Country()
                    {
                        CountryId = Guid.Parse("6E3BE723-13E2-4068-B392-D9353ED41F6D"),
                        CountryName = "Germany"
                    },
                    new Country()
                    {
                        CountryId = Guid.Parse("FDDE1F72-2A86-4A39-9ECC-846D840B91B9"),
                        CountryName = "Canada"
                    },
                    new Country()
                    {
                        CountryId = Guid.Parse("13C250B3-2AF6-40A9-9616-6E5ABF1EA2A9"),
                        CountryName = "Costa Rica"
                    },
                    new Country()
                    {
                        CountryId = Guid.Parse("969874C5-A77D-4158-B033-605FF940D04E"),
                        CountryName = "Chile"
                    },
                    new Country()
                    {
                        CountryId = Guid.Parse("D67524D3-BF47-4F48-AEF1-E20BBE3F0443"),
                        CountryName = "Ireland"
                    },
                });
            }
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

        public List<CountryResponse> GetCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null) return null;

            return _countries.FirstOrDefault(country => country.CountryId.Equals(countryId))?.ToCountryResponse();
        }
    }
}
