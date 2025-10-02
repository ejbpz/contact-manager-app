using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.Services;
using AutoFixture;
using FluentAssertions;
using ContactsManager.RepositoryContracts;
using Moq;

namespace ContactsManager.Test
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        private readonly Mock<ICountriesRepository> _mockCountriesRepository;
        private readonly ICountriesRepository _countriesRepository;

        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _mockCountriesRepository = new Mock<ICountriesRepository>();
            _countriesRepository = _mockCountriesRepository.Object;

            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        // When CountryAddRequest is null (ArgumentNullException).
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
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
        public async Task AddCountry_NullName_ToBeArgumentException()
        {
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When the CountryName is duplicate (ArgumentException).
        [Fact]
        public async Task AddCountry_DuplicatedName_ToBeArgumentException()
        {
            // Arrange
            Country country1 = _fixture.Build<Country>()
                .With(c => c.CountryName, "Costa Rica")
                .Create();

            Country country2 = _fixture.Build<Country>()
                .With(c => c.CountryName, "Costa Rica")
                .Create();

            _mockCountriesRepository.Setup(m => m
                .AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);

            _mockCountriesRepository.Setup(m => m
                .GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            // Act
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(country1.ToCountryAddRequest());

                _mockCountriesRepository.Setup(m => m
                    .GetCountryByCountryName(It.IsAny<string>()))
                    .ReturnsAsync(country1);

                await _countriesService.AddCountry(country2.ToCountryAddRequest());
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When you supply proper CountryName (insert the country).
        [Fact]
        public async Task AddCountry_ProperName_ToBeSuccessful()
        {
            // Arrange
            Country country = _fixture.Create<Country>();
            CountryResponse countryExpected = country.ToCountryResponse();

            _mockCountriesRepository.Setup(m => m
                .AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            _mockCountriesRepository.Setup(m => m
                .GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            // Act
            CountryResponse countryResponse = await _countriesService.AddCountry(country.ToCountryAddRequest());
            countryResponse.CountryId = country.CountryId;

            // Assert
            countryResponse.CountryId.Should().NotBe(Guid.Empty);
            countryResponse.Should().Be(countryExpected);
        }
        #endregion

        #region GetCountries
        // When the list of countries is empty by default.
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmptyList()
        {
            // Act
            _mockCountriesRepository.Setup(m => m
                .GetCountries())
                .ReturnsAsync(new List<Country>());

            List<CountryResponse> actualCountryList = await _countriesService.GetCountries();

            // Assert
            actualCountryList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            IEnumerable<Country> countries = _fixture.CreateMany<Country>();
            List<CountryResponse> countriesExpected = countries.Select(c => c.ToCountryResponse()).ToList();

            _mockCountriesRepository.Setup(m => m
                .GetCountries())
                .ReturnsAsync(countries.ToList());

            // Act
            List<CountryResponse> allCountries = await _countriesService.GetCountries();

            // Assert
            allCountries.Should().BeEquivalentTo(countriesExpected);
        }
        #endregion

        #region GetCountryById
        // When the CountryId is null, return null as a CountryResponse?
        [Fact]
        public async Task GetCountryById_NullCountryId_ToBeNull()
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
        public async Task GetCountryById_ProperId_ToBeSuccessful()
        {
            // Arrange
            Country country = _fixture.Create<Country>();
            CountryResponse countryExpected = country.ToCountryResponse();

            _mockCountriesRepository.Setup(m => m
                .GetCountryByCountryId(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(country.CountryId);

            // Assert
            countryResponse.Should().Be(countryExpected);
        }
        #endregion
    }
}
