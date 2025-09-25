using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace ContactsManager.Test
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            List<Country> countries = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
            );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countries);

            _countriesService = new CountriesService(null);
        }

        #region AddCountry
        // When CountryAddRequest is null (ArgumentNullException).
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            Func<Task> action = async () =>
            {
                // Act
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When the CountryName is null (ArgumentException).
        [Fact]
        public async Task AddCountry_NullName()
        {
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, value: null)
                .Create();

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When the CountryName is duplicate (ArgumentException).
        [Fact]
        public async Task AddCountry_DuplicatedName()
        {
            IEnumerable<CountryAddRequest> requests = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "Costa Rica")
                .CreateMany(2);

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(requests.First());
                await _countriesService.AddCountry(requests.Last());
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When you supply proper CountryName (insert the country).
        [Fact]
        public async Task AddCountry_ProperName()
        {
            // Arrange
            CountryAddRequest request = _fixture.Create<CountryAddRequest>();

            // Act
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> actualAllProducts = await _countriesService.GetCountries();

            // Assert
            countryResponse.CountryId.Should().NotBe(Guid.Empty);
            actualAllProducts.Should().Contain(countryResponse);
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
            actualCountryList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            IEnumerable<CountryAddRequest> actualCountryList = _fixture.CreateMany<CountryAddRequest>();
            List<CountryResponse> countryResponses = new List<CountryResponse>();

            // Act
            foreach (CountryAddRequest countryAddRequest in actualCountryList)
            {
                countryResponses.Add(await _countriesService.AddCountry(countryAddRequest));
            }

            List<CountryResponse> allCountries = await _countriesService.GetCountries();

            // Assert
            allCountries.Should().BeEquivalentTo(countryResponses);
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
            countryResponse.Should().BeNull();
        }

        // When we supply a valid CountryId, it should return a CountryResponse.
        [Fact]
        public async Task GetCountryById_ProperId()
        {
            // Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

            // Act
            CountryResponse expectedResponse = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(expectedResponse.CountryId);

            // Assert
            countryResponse.Should().Be(expectedResponse);
        }
        #endregion
    }
}
