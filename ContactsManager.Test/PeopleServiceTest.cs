using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services;
using Xunit.Abstractions;

namespace ContactsManager.Test
{
    public class PeopleServiceTest
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PeopleServiceTest(ITestOutputHelper testOutputHelper)
        {
            _peopleService = new PeopleService();
            _countryService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        // When we supply a null value (ArgumentNullException).
        [Fact]
        public void AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _peopleService.AddPerson(personAddRequest);
            });
        }

        // When we supply a attribute (PersonName) null value (ArgumentException).
        [Fact]
        public void AddPerson_NullPersonName()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _peopleService.AddPerson(personAddRequest);
            });
        }

        // When we supply a correct PersonAddRequest value and should be in the list of people.
        [Fact]
        public void AddPerson_ProperPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Eduardo J. Brenes Perez",
                PersonEmail = "email@sample.com",
                DateOfBirth = new DateTime(2004, 9, 8),
                Gender = GenderOptions.Male,
                CountryId = Guid.NewGuid(),
                Address = "1st Street, Costa Rica",
                IsReceivingNewsLetters = true,
            };
            PersonResponse personRetrieved = new PersonResponse();
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act
            personRetrieved = _peopleService.AddPerson(personAddRequest);
            listOfPeople = _peopleService.GetPeople();

            // Assert
            Assert.True(personRetrieved.PersonId != Guid.Empty);
            Assert.Contains(personRetrieved, listOfPeople);
        }
        #endregion

        #region GetPerson
        // When the person ID is not supplied, it should return null.
        [Fact]
        public void GetPerson_NullGuid()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? personResponse = _peopleService.GetPersonByPersonId(personId);

            // Assert
            Assert.Null(personResponse);
        }

        // If we supply a valid personId, it should return a valid PersonResponse.
        [Fact]
        public void GetPerson_ValidPersonId()
        {
            // Arrange Country
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Costa Rica"
            };
            CountryResponse? countryAdded = new CountryResponse();

            // Act Country
            countryAdded = _countryService.AddCountry(countryAddRequest);

            // Arrange Person
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Eduardo J. Brenes Perez",
                PersonEmail = "email@sample.com",
                DateOfBirth = new DateTime(2004, 9, 8),
                Gender = GenderOptions.Male,
                CountryId = countryAdded.CountryId,
                Address = "1st Street, Costa Rica",
                IsReceivingNewsLetters = true,
            };
            PersonResponse? personAdded = new PersonResponse();
            PersonResponse? personSearched = new PersonResponse();
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act Person
            personAdded = _peopleService.AddPerson(personAddRequest);
            personSearched = _peopleService.GetPersonByPersonId(personAdded.PersonId);
            listOfPeople = _peopleService.GetPeople();

            // Assert
            Assert.Equal(personAdded, personSearched);
            Assert.Contains(personSearched, listOfPeople);
        }
        #endregion

        #region GetPeople
        // The list of people is empty
        [Fact]
        public void GetPeople_EmptyList()
        {
            // Arrange
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act
            listOfPeople = _peopleService.GetPeople();

            // Assert
            Assert.Empty(listOfPeople);
        }

        // We'll need to receive all the people which we added.
        [Fact]
        public void GetPeople_AddPeople()
        {
            // Arrange Country
            CountryAddRequest? countryAddRequest1 = new CountryAddRequest()
            {
                CountryName = "Costa Rica"
            };
            CountryAddRequest? countryAddRequest2 = new CountryAddRequest()
            {
                CountryName = "Canada"
            };
            CountryAddRequest? countryAddRequest3 = new CountryAddRequest()
            {
                CountryName = "Belgium"
            };
            CountryAddRequest? countryAddRequest4 = new CountryAddRequest()
            {
                CountryName = "Panama"
            };
            CountryAddRequest? countryAddRequest5 = new CountryAddRequest()
            {
                CountryName = "Mexico"
            };
            CountryResponse? countryAdded1 = new CountryResponse();
            CountryResponse? countryAdded2 = new CountryResponse();
            CountryResponse? countryAdded3 = new CountryResponse();
            CountryResponse? countryAdded4 = new CountryResponse();
            CountryResponse? countryAdded5 = new CountryResponse();

            // Act Country
            countryAdded1 = _countryService.AddCountry(countryAddRequest1);
            countryAdded2 = _countryService.AddCountry(countryAddRequest2);
            countryAdded3 = _countryService.AddCountry(countryAddRequest3);
            countryAdded4 = _countryService.AddCountry(countryAddRequest4);
            countryAdded5 = _countryService.AddCountry(countryAddRequest5);

            // Arrange Person
            PersonAddRequest? personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Person 1",
                PersonEmail = "email1@sample.com",
                DateOfBirth = new DateTime(2023, 9, 23),
                Gender = GenderOptions.Male,
                CountryId = countryAdded1.CountryId,
                Address = "Person 1 address",
                IsReceivingNewsLetters = true,
            };
            PersonAddRequest? personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Person 2",
                PersonEmail = "email2@sample.com",
                DateOfBirth = new DateTime(1994, 2, 8),
                Gender = GenderOptions.Other,
                CountryId = countryAdded2.CountryId,
                Address = "Person 2 address",
                IsReceivingNewsLetters = false,
            };
            PersonAddRequest? personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Person 3",
                PersonEmail = "email3@sample.com",
                DateOfBirth = new DateTime(2004, 9, 22),
                Gender = GenderOptions.Female,
                CountryId = countryAdded3.CountryId,
                Address = "Person 3 address",
                IsReceivingNewsLetters = true,
            };
            PersonAddRequest? personAddRequest4 = new PersonAddRequest()
            {
                PersonName = "Person 4",
                PersonEmail = "email4@sample.com",
                DateOfBirth = new DateTime(2012, 12, 31),
                Gender = GenderOptions.Male,
                CountryId = countryAdded4.CountryId,
                Address = "Person 4 address",
                IsReceivingNewsLetters = true,
            };
            PersonAddRequest? personAddRequest5 = new PersonAddRequest()
            {
                PersonName = "Person 5",
                PersonEmail = "email5@sample.com",
                DateOfBirth = new DateTime(1987, 4, 15),
                Gender = GenderOptions.Female,
                CountryId = countryAdded5.CountryId,
                Address = "Person 5 address",
                IsReceivingNewsLetters = false,
            };
            List<PersonAddRequest> addPeople = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3,
                personAddRequest4,
                personAddRequest5,
            };
            List<PersonResponse>? listOfPeopleAdded = new List<PersonResponse>();
            List<PersonResponse> listOfPeople = new List<PersonResponse>();

            // Act Person
            foreach (PersonAddRequest personAddRequest in addPeople)
            {
                listOfPeopleAdded.Add(_peopleService.AddPerson(personAddRequest));
            }
            listOfPeople = _peopleService.GetPeople();

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
    }
}
