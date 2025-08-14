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

        #region AddCountry
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
            List<CountryResponse> actualAllProducts = _countriesService.GetCountries();

            // Assert
            Assert.True(countryResponse.CountryId != Guid.Empty);
            Assert.Contains(countryResponse, actualAllProducts);
        }
        #endregion

        #region GetCountries
        // When the list of countries is empty by default.
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            // Act
            List<CountryResponse> actualCountryList = _countriesService.GetCountries();

            // Assert
            Assert.Empty(actualCountryList);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> actualCountryList = new List<CountryAddRequest>()
            {
                new CountryAddRequest()
                {
                    CountryName = "Germany"
                },
                new CountryAddRequest()
                {
                    CountryName = "Canada"
                },
                new CountryAddRequest()
                {
                    CountryName = "Costa Rica"
                },
            };

            List<CountryResponse> countryResponses = new List<CountryResponse>();

            // Act
            foreach (CountryAddRequest countryAddRequest in actualCountryList)
            {
                countryResponses.Add(_countriesService.AddCountry(countryAddRequest));
            }

            List<CountryResponse> allCountries = _countriesService.GetCountries();

            // Assert
            foreach (CountryResponse expectedCountry in countryResponses)
            {
                Assert.Contains(expectedCountry, allCountries);
            }
        }
        #endregion

        #region GetCountryById
        // When the CountryId is null, return null as a CountryResponse?
        [Fact]
        public void GetCountryById_NullCountryId()
        {
            // Arrange
            Guid? countryId = null;

            // Act
            CountryResponse? countryResponse = _countriesService.GetCountryByCountryId(countryId);

            // Assert
            Assert.Null(countryResponse);
        }

        // When we supply a valid CountryId, it should return a CountryResponse.
        [Fact]
        public void GetCountryById_ProperId()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Costa Rica",
            };

            // Act
            CountryResponse expectedResponse = _countriesService.AddCountry(countryAddRequest);
            CountryResponse? countryResponse = _countriesService.GetCountryByCountryId(expectedResponse.CountryId);

            // Assert
            Assert.Equal(expectedResponse, countryResponse);
        }
        #endregion
    }
}
