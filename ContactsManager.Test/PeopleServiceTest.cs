using AutoFixture;
using ContactsManager.Models;
using ContactsManager.Repository;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ContactsManager.Test
{
    public class PeopleServiceTest
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countryService;

        private readonly Mock<IPeopleRepository> _mockPeopleRepository;
        private readonly IPeopleRepository _peopleRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PeopleServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _mockPeopleRepository = new Mock<IPeopleRepository>();
            _peopleRepository = _mockPeopleRepository.Object;

            //List<Person> people = new List<Person>() { };
            //List<Country> countries = new List<Country>() { };

            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //    new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //);

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock<Person>(temp => temp.People, people);
            //dbContextMock.CreateDbSetMock<Country>(temp => temp.Countries, countries);


            _countryService = new CountriesService(null);
            _peopleService = new PeopleService(_peopleRepository);
            _testOutputHelper = testOutputHelper;
        }

        private List<Person> AddingPeople()
        {
            IEnumerable<Person> people = _fixture.Build<Person>()
                .With(p => p.Country, null as Country)
                .CreateMany(5);

            return people.ToList();
        }

        #region AddPerson
        // When we supply a null value (ArgumentNullException).
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Assert
            Func<Task> action = async () =>
            {
                // Act
                await _peopleService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When we supply a attribute (PersonName) null value (ArgumentException).
        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonEmail, value: "email@example.com")
                .With(p => p.PersonName, value: null)
                .Create();

            // Assert
            Func<Task> action = async () =>
            {
                // Act
                await _peopleService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When we supply a correct PersonAddRequest value and should be in the list of people.
        [Fact]
        public async Task AddPerson_ProperPerson_ToBeSuccessful()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonEmail, "email@sample.com")
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personExpected = person.ToPersonResponse();

            _mockPeopleRepository.Setup(m => m
                .AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act
            PersonResponse personRetrieved = await _peopleService.AddPerson(personAddRequest);
            personExpected.PersonId = personRetrieved.PersonId;

            // Assert
            personRetrieved.PersonId.Should().NotBe(Guid.Empty);
            personRetrieved.Should().Be(personExpected);
        }
        #endregion

        #region GetPerson
        // When the person ID is not supplied, it should return null.
        [Fact]
        public async Task GetPerson_NullGuid_ToBeNull()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(personId);

            // Assert
            personResponse.Should().BeNull();
        }

        // If we supply a valid personId, it should return a valid PersonResponse.
        [Fact]
        public async Task GetPerson_ValidPersonId_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonEmail, "email@sample.com")
                .Create();

            PersonResponse personExpected = person.ToPersonResponse();

            _mockPeopleRepository.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            // Act
            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(person.PersonId);

            // Assert
            personResponse.Should().Be(personExpected);
        }
        #endregion

        #region GetPeople
        // The list of people is empty
        [Fact]
        public async Task GetPeople_EmptyList_ToBeEmpty()
        {
            // Arrange
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            _mockPeopleRepository.Setup(m => m
                .GetPeople())
                .ReturnsAsync(new List<Person>());

            // Act
            listOfPeople = await _peopleService.GetPeople();

            // Assert
            listOfPeople.Should().BeEmpty();
        }

        // We'll need to receive all the people which we added.
        [Fact]
        public async Task GetPeople_AddPeople_ToBeSuccessful()
        {
            // Arrange
            List<Person> listOfPeople = AddingPeople();

            List<PersonResponse> peopleExpected = listOfPeople.Select(p => p.ToPersonResponse()).ToList();

            _mockPeopleRepository.Setup(m => m
                .GetPeople())
                .ReturnsAsync(listOfPeople);

            // Act
            List<PersonResponse> peopleResponse = await _peopleService.GetPeople();

            // Print people added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in peopleExpected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _testOutputHelper.WriteLine("\nActual:");
            foreach (PersonResponse? person in peopleResponse)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            // Assert
            peopleResponse.Should().BeEquivalentTo(peopleExpected);
        }
        #endregion

        #region GetFilteredPeople
        // If the search is not supplied, it'll return every person.
        [Fact]
        public async Task GetFilteredPeople_EmptySearch_ToNotBeFiltered()
        {
            // Arrange
            List<Person> listOfPeople = AddingPeople();
            List<PersonResponse> peopleExpected = listOfPeople.Select(p => p.ToPersonResponse()).ToList();

            _mockPeopleRepository.Setup(m => m
                .GetFilteredPeople(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(listOfPeople);

            // Act
            List<PersonResponse> peopleResponse = await _peopleService.GetFilteredPeople(nameof(Person.PersonName), "");

            // Print
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in peopleExpected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _testOutputHelper.WriteLine("\nFiltered:");
            foreach (PersonResponse people in peopleResponse)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            // Assert
            peopleResponse.Should().BeEquivalentTo(peopleExpected);
        }

        // We'll add a few persons, then we are going to search based on name and a query.
        [Fact]
        public async Task GetFilteredPeople_SearchByName_ToBeSuccessful()
        {
            // Arrange
            List<Person> listOfPeople = AddingPeople();
            List<PersonResponse> peopleExpected = listOfPeople.Where(p => p.PersonName!.Contains("a")).Select(p => p.ToPersonResponse()).ToList();

            _mockPeopleRepository.Setup(m => m
                .GetFilteredPeople(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(listOfPeople.Where(p => p.PersonName!.Contains("a")).ToList());

            // Act
            List<PersonResponse> peopleResponse = await _peopleService.GetFilteredPeople(nameof(Person.PersonName), "a");

            // Print
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in peopleExpected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _testOutputHelper.WriteLine("\nFiltered:");
            foreach (PersonResponse people in peopleResponse)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            // Assert
            peopleResponse.Should().BeEquivalentTo(peopleExpected);
        }
        #endregion

        #region GetSortedPeople
        // When we sort based on PersonName in descending order. It should return a List with this preferences.
        [Fact]
        public async Task GetSortedPeople_DescendingNameSort_ToBeSuccessful()
        {
            // Arrange
            List<Person> listOfPeople = AddingPeople();

            _mockPeopleRepository.Setup(m => m
                .GetPeople())
                .ReturnsAsync(listOfPeople);

            // Act
            List<PersonResponse> peopleResponse = await _peopleService.GetPeople();

            List<PersonResponse> peopleSorted = _peopleService.GetSortedPeople(peopleResponse, nameof(Person.PersonName), SortOrderOptions.Descending);

            // Print
            _testOutputHelper.WriteLine("People (not sorted):");
            foreach (PersonResponse? person in peopleResponse)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            _testOutputHelper.WriteLine("\nPeople (sorted):");
            foreach (PersonResponse? people in peopleSorted)
            {
                _testOutputHelper.WriteLine(people.ToString());
            }

            peopleSorted.Should().BeInDescendingOrder(p => p.PersonName);
        }
        #endregion

        #region UpdatePerson
        // If we supply a null value, should throw an exception (ArgumentNullException).
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? personUpdate = null;

            // Assert
            Func<Task> action = async () =>
            {
                // Act
                await _peopleService.UpdatePerson(personUpdate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // If we supply a null person id or not known id, should throw an exception (ArgumentException).
        [Fact]
        public async Task UpdatePerson_NullPersonId_ToBeArgumentException()
        {
            // Arrange
            PersonUpdateRequest personUpdate = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.PersonId, value: Guid.Empty)
                .Create();

            // Assert
            Func<Task> action = async () =>
            {
                // Act
                await _peopleService.UpdatePerson(personUpdate);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // If we supply a null person name, should throw an exception (ArgumentException).
        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonName, null as string)
                .With(p => p.Country, null as Country)
                .With(p => p.PersonEmail, "sample@email.com")
                .Create();

            // Assert
            Func<Task> action = async () =>
            {
                // Act
                await _peopleService.UpdatePerson(person.ToPersonResponse().ToPersonUpdateRequest());
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        // If we supply a null person name, should throw an exception (ArgumentException).
        [Fact]
        public async Task UpdatePerson_ProperPerson_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonName, "Eduardo")
                .With(p => p.PersonEmail, "sample@email.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, GenderOptions.Male.ToString())
                .Create();

            PersonResponse personExpected = person.ToPersonResponse();
            PersonUpdateRequest personUpdate = personExpected.ToPersonUpdateRequest();

            _mockPeopleRepository.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _mockPeopleRepository.Setup(m => m
                .UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act
            PersonResponse? personResponse = await _peopleService.UpdatePerson(personUpdate);

            // Assert
            personResponse.Should().Be(personExpected);
        }
        #endregion

        #region DeletePerson
        // If we supply an invalid PersonId (null | it does not exist), it should return false.
        [Fact]
        public async Task DeletePerson_WrongId_ToBeFalse()
        {
            // Act
            bool wasDeleted1 = await _peopleService.DeletePerson(Guid.NewGuid());
            bool wasDeleted2 = await _peopleService.DeletePerson(null);

            // Assert
            wasDeleted1.Should().BeFalse();
            wasDeleted2.Should().BeFalse();
        }

        // If we supply a valid PersonId (exist), it should return true;
        [Fact]
        public async Task DeletePerson_ProperId_ToBeTrue()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonName, "Eduardo")
                .With(p => p.PersonEmail, "sample@email.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, GenderOptions.Male.ToString())
                .Create();

            _mockPeopleRepository.Setup(m => m
                .DeletePersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            bool wasDeleted = await _peopleService.DeletePerson(person.PersonId);

            // Assert
            wasDeleted.Should().BeTrue();
        }
        #endregion
    }
}
