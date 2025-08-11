using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.Services;

namespace ContactsManager.Test
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        // When CountryAddRequest is null (ArgumentNullException).
        [Fact]
        public void AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _countriesService.AddCountry(request);
            });
        }

        // When the CountryName is null (ArgumentException).
        [Fact]
        public void AddCountry_NullName()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null,
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }

        // When the CountryName is duplicate (ArgumentException).
        [Fact]
        public void AddCountry_DuplicatedName()
        {
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "Costa Rica",
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "Costa Rica",
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        // When you supply proper CountryName (insert the country).
        [Fact]
        public void AddCountry_ProperName()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Canada",
            };

            // Act
            CountryResponse countryResponse = _countriesService.AddCountry(request);

            // Assert
            Assert.True(countryResponse.CountryId != Guid.Empty);
        }
    }
}
