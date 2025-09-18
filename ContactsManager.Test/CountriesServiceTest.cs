using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;

namespace ContactsManager.Test
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            List<Country> countries = new List<Country>()
            {

            };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
            );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countries);

            _countriesService = new CountriesService(dbContext);
        }

        #region AddCountry
        // When CountryAddRequest is null (ArgumentNullException).
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(request);
            });
        }

        // When the CountryName is null (ArgumentException).
        [Fact]
        public async Task AddCountry_NullName()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null,
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        // When the CountryName is duplicate (ArgumentException).
        [Fact]
        public async Task AddCountry_DuplicatedName()
        {
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "Costa Rica",
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "Costa Rica",
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }

        // When you supply proper CountryName (insert the country).
        [Fact]
        public async Task AddCountry_ProperName()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Canada",
            };

            // Act
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> actualAllProducts = await _countriesService.GetCountries();

            // Assert
            Assert.True(countryResponse.CountryId != Guid.Empty);
            Assert.Contains(countryResponse, actualAllProducts);
        }
        #endregion

        #region GetCountries
        // When the list of countries is empty by default.
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // Act
            List<CountryResponse> actualCountryList = await _countriesService.GetCountries();

            // Assert
            Assert.Empty(actualCountryList);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
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
                countryResponses.Add(await _countriesService.AddCountry(countryAddRequest));
            }

            List<CountryResponse> allCountries = await _countriesService.GetCountries();

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
        public async Task GetCountryById_NullCountryId()
        {
            // Arrange
            Guid? countryId = null;

            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(countryId);

            // Assert
            Assert.Null(countryResponse);
        }

        // When we supply a valid CountryId, it should return a CountryResponse.
        [Fact]
        public async Task GetCountryById_ProperId()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Costa Rica",
            };

            // Act
            CountryResponse expectedResponse = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(expectedResponse.CountryId);

            // Assert
            Assert.Equal(expectedResponse, countryResponse);
        }
        #endregion
    }
}
