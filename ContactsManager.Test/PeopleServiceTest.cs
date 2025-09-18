using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace ContactsManager.Test
{
    public class PeopleServiceTest
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PeopleServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            List<Person> people = new List<Person>() { };
            List<Country> countries = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
            );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock<Person>(temp => temp.People, people);
            dbContextMock.CreateDbSetMock<Country>(temp => temp.Countries, countries);


            _countryService = new CountriesService(dbContext);
            _peopleService = new PeopleService(dbContext);
            _testOutputHelper = testOutputHelper;
        }

        private async Task<List<CountryResponse>> AddingCountries()
        {
            IEnumerable<CountryAddRequest> countriesToAdd = _fixture.CreateMany<CountryAddRequest>(5);
            
            List<CountryResponse> countriesAdded = new List<CountryResponse>();

            foreach (CountryAddRequest countryAddRequest in countriesToAdd) countriesAdded.Add(await _countryService.AddCountry(countryAddRequest));

            return countriesAdded;
        }

        private async Task<List<PersonResponse>> AddingPeople(List<CountryResponse> countriesAdded)
        {
            IEnumerable<PersonAddRequest> peopleToAdd = _fixture.Build<PersonAddRequest>()
                .FromFactory(() =>
                {
                    var country = countriesAdded[Random.Shared.Next(countriesAdded.Count)];
                    return new PersonAddRequest { CountryId = country.CountryId };
                })
                .CreateMany(5);

            List<PersonResponse>? listOfPeopleAdded = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in peopleToAdd)
            {
                listOfPeopleAdded.Add(await _peopleService.AddPerson(personAddRequest));
            }

            return listOfPeopleAdded;
        }

        #region AddPerson
        // When we supply a null value (ArgumentNullException).
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _peopleService.AddPerson(personAddRequest);
            });
        }

        // When we supply a attribute (PersonName) null value (ArgumentException).
        [Fact]
        public async Task AddPerson_NullPersonName()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonEmail, value: "email@example.com")
                .With(p => p.PersonName, value: null)
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _peopleService.AddPerson(personAddRequest);
            });
        }

        // When we supply a correct PersonAddRequest value and should be in the list of people.
        [Fact]
        public async Task AddPerson_ProperPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonEmail, "email@sample.com")
                .Create();

            PersonResponse personRetrieved = new PersonResponse();
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act
            personRetrieved = await _peopleService.AddPerson(personAddRequest);
            listOfPeople = await _peopleService.GetPeople();

            // Assert
            Assert.True(personRetrieved.PersonId != Guid.Empty);
            Assert.Contains(personRetrieved, listOfPeople);
        }
        #endregion

        #region GetPerson
        // When the person ID is not supplied, it should return null.
        [Fact]
        public async Task GetPerson_NullGuid()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(personId);

            // Assert
            Assert.Null(personResponse);
        }

        // If we supply a valid personId, it should return a valid PersonResponse.
        [Fact]
        public async Task GetPerson_ValidPersonId()
        {
            // Arrange Country
            CountryAddRequest? countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse? countryAdded = new CountryResponse();

            // Act Country
            countryAdded = await _countryService.AddCountry(countryAddRequest);

            // Arrange Person
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonEmail, "email@sample.com")
                .With(p => p.CountryId, countryAdded.CountryId)
                .Create();
            PersonResponse? personAdded = new PersonResponse();
            PersonResponse? personSearched = new PersonResponse();
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act Person
            personAdded = await _peopleService.AddPerson(personAddRequest);
            personSearched = await _peopleService.GetPersonByPersonId(personAdded.PersonId);
            listOfPeople = await _peopleService.GetPeople();

            // Assert
            Assert.Equal(personAdded, personSearched);
            Assert.Contains(personSearched, listOfPeople);
        }
        #endregion

        #region GetPeople
        // The list of people is empty
        [Fact]
        public async Task GetPeople_EmptyList()
        {
            // Arrange
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act
            listOfPeople = await _peopleService.GetPeople();

            // Assert
            Assert.Empty(listOfPeople);
        }

        // We'll need to receive all the people which we added.
        [Fact]
        public async Task GetPeople_AddPeople()
        {
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> listOfPeopleAdded = await AddingPeople(countriesAdded);
            List<PersonResponse> listOfPeople = await _peopleService.GetPeople();

            // Print people added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                _testOutputHelper.WriteLine(personAdded.ToString());
            }

            _testOutputHelper.WriteLine("\nActual:");
            foreach (PersonResponse? people in listOfPeople)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            // Assert
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                Assert.Contains(personAdded, listOfPeople);
            }
        }
        #endregion

        #region GetFilteredPeople
        // If the search is not supplied, it'll return every person.
        [Fact]
        public async Task GetFilteredPeople_EmptySearch()
        {
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> listOfPeopleAdded = await AddingPeople(countriesAdded);
            List<PersonResponse> listOfPeople = await _peopleService.GetFilteredPeople(nameof(Person.PersonName), "");

            // Print people added
            _testOutputHelper.WriteLine("People already added:");
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                _testOutputHelper.WriteLine(personAdded.ToString());
            }

            _testOutputHelper.WriteLine("\nFiltered:");
            foreach (PersonResponse? people in listOfPeople)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            // Assert
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                Assert.Contains(personAdded, listOfPeople);
            }
        }

        // We'll add a few persons, then we are going to search based on name and a query.
        [Fact]
        public async Task GetFilteredPeople_SearchByGender()
        {
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> listOfPeopleAdded = await AddingPeople(countriesAdded);
            List<PersonResponse>? listOfPeople = await _peopleService.GetFilteredPeople(nameof(Person.Gender), "Other");

            // Print people added
            _testOutputHelper.WriteLine("People already added:");
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                _testOutputHelper.WriteLine(personAdded.ToString());
            }

            _testOutputHelper.WriteLine("\nFiltered:");
            foreach (PersonResponse? people in listOfPeople)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            // Assert
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                if(personAdded.Gender is not null)
                {
                    if (personAdded.Gender.Contains("Other", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personAdded, listOfPeople);
                    }
                }
            }
        }
        #endregion

        #region GetSortedPeople
        // When we sort based on PersonName in descending order. It should return a List with this preferences.
        [Fact]
        public async Task GetSortedPeople_EmptySearch()
        {
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> listOfPeopleAdded = await AddingPeople(countriesAdded);

            List<PersonResponse> listOfPeopleSorted = _peopleService.GetSortedPeople(listOfPeopleAdded, nameof(Person.PersonName), SortOrderOptions.Descending);

            // Print people added
            _testOutputHelper.WriteLine("People added (actual):");
            foreach (PersonResponse? personAdded in listOfPeopleAdded)
            {
                _testOutputHelper.WriteLine(personAdded.ToString());
            }

            _testOutputHelper.WriteLine("\nPeople sorted (service):");
            foreach (PersonResponse? people in listOfPeopleSorted)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            listOfPeopleAdded = listOfPeopleAdded.OrderByDescending(person => person.PersonName).ToList();
            _testOutputHelper.WriteLine("\nPeople sorted (actual):");
            foreach (PersonResponse? people in listOfPeopleAdded)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }
            // Assert
            for (int i = 0; i < listOfPeopleSorted.Count; i++)
            {
                Assert.Equal(listOfPeopleSorted[i], listOfPeopleAdded[i]);
            }
        }
        #endregion

        #region UpdatePerson
        // If we supply a null value, should throw an exception (ArgumentNullException).
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? personUpdate = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _peopleService.UpdatePerson(personUpdate);
            });
        }

        // If we supply a null person id or not known id, should throw an exception (ArgumentException).
        [Fact]
        public async Task UpdatePerson_NullPersonId()
        {
            // Arrange
            CountryResponse countryAdded = _fixture.Create<CountryResponse>();

            PersonUpdateRequest personUpdate = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.CountryId, value: countryAdded?.CountryId)
                .With(p => p.PersonId, value: Guid.Empty)
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _peopleService.UpdatePerson(personUpdate);
            });
        }

        // If we supply a null person name, should throw an exception (ArgumentException).
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // Arrange
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> peopleAdded = await AddingPeople(countriesAdded);

            PersonUpdateRequest? personUpdate = peopleAdded[0].ToPersonUpdateRequest();
            personUpdate.PersonName = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _peopleService.UpdatePerson(personUpdate);
            });
        }

        // If we supply a null person name, should throw an exception (ArgumentException).
        [Fact]
        public async Task UpdatePerson_ProperPerson()
        {
            // Arrange
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> peopleAdded = await AddingPeople(countriesAdded);

            PersonUpdateRequest? personUpdate = peopleAdded[0].ToPersonUpdateRequest();
            personUpdate.PersonName = "Eduardo";
            personUpdate.PersonEmail = "email@example.com";

            // Act
            PersonResponse personResponse = await _peopleService.UpdatePerson(personUpdate);

            PersonResponse? personExpected = await _peopleService.GetPersonByPersonId(personResponse.PersonId);

            // Assert
            Assert.Equal(personExpected, personResponse);
        }
        #endregion

        #region DeletePerson
        // If we supply an invalid PersonId (null | it does not exist), it should return false.
        [Fact]
        public async Task DeletePerson_WrongId()
        {
            // Act
            bool wasDeleted1 = await _peopleService.DeletePerson(Guid.NewGuid());
            bool wasDeleted2 = await _peopleService.DeletePerson(null);

            // Assert
            Assert.False(wasDeleted1);
            Assert.False(wasDeleted2);
        }

        // If we supply a valid PersonId (exist), it should return true;
        [Fact]
        public async Task DeletePerson_ProperId()
        {
            // Arrange
            List<CountryResponse> countriesAdded = await AddingCountries();
            List<PersonResponse> peopleAdded = await AddingPeople(countriesAdded);
            List<bool> wereDeleted = new List<bool>();

            // Act
            foreach (PersonResponse personResponse in peopleAdded)
            {
                wereDeleted.Add(await _peopleService.DeletePerson(personResponse.PersonId));
            }

            // Assert
            foreach (bool wasDeleted in wereDeleted)
            {
                Assert.True(wasDeleted);
            }
        }
        #endregion
    }
}
