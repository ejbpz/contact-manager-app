using ContactsManager.Models;
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

        private List<CountryResponse?> AddingCountries()
        {
            CountryAddRequest? countryAddRequest1 = new CountryAddRequest() { CountryName = "Costa Rica" };
            CountryAddRequest? countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };
            CountryAddRequest? countryAddRequest3 = new CountryAddRequest() { CountryName = "Belgium" };
            CountryAddRequest? countryAddRequest4 = new CountryAddRequest() { CountryName = "Panama" };
            CountryAddRequest? countryAddRequest5 = new CountryAddRequest() { CountryName = "Mexico" };

            List<CountryAddRequest> countriesToAdd = new List<CountryAddRequest>()
            {
                countryAddRequest1,
                countryAddRequest2,
                countryAddRequest3,
                countryAddRequest4,
                countryAddRequest5,
            }; 
            
            List<CountryResponse?> countriesAdded = new List<CountryResponse?>();

            foreach (CountryAddRequest countryAddRequest in countriesToAdd) countriesAdded.Add(_countryService.AddCountry(countryAddRequest));

            return countriesAdded;
        }

        private List<PersonResponse> AddingPeople(List<CountryResponse?> countriesAdded)
        {
            PersonAddRequest? personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Pedro",
                PersonEmail = "pedro@gmail.com",
                DateOfBirth = new DateTime(2023, 9, 23),
                Gender = GenderOptions.Male,
                CountryId = countriesAdded[0]?.CountryId,
                Address = "Person 1 address",
                IsReceivingNewsLetters = true,
            };
            PersonAddRequest? personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Joao",
                PersonEmail = "joao@hotmail.com",
                DateOfBirth = new DateTime(1994, 2, 8),
                Gender = GenderOptions.Other,
                CountryId = countriesAdded[1]?.CountryId,
                Address = "Person 2 address",
                IsReceivingNewsLetters = false,
            };
            PersonAddRequest? personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Maria",
                PersonEmail = "mary@live.com",
                DateOfBirth = new DateTime(2004, 9, 22),
                Gender = GenderOptions.Female,
                CountryId = countriesAdded[2]?.CountryId,
                Address = "Person 3 address",
                IsReceivingNewsLetters = true,
            };
            PersonAddRequest? personAddRequest4 = new PersonAddRequest()
            {
                PersonName = "Roman",
                PersonEmail = "rom32@outlook.com",
                DateOfBirth = new DateTime(2012, 12, 31),
                Gender = GenderOptions.Male,
                CountryId = countriesAdded[3]?.CountryId,
                Address = "Person 4 address",
                IsReceivingNewsLetters = true,
            };
            PersonAddRequest? personAddRequest5 = new PersonAddRequest()
            {
                PersonName = "Karla",
                PersonEmail = "karla2@gmail.com",
                DateOfBirth = new DateTime(1987, 4, 15),
                Gender = GenderOptions.Female,
                CountryId = countriesAdded[4]?.CountryId,
                Address = "Person 5 address",
                IsReceivingNewsLetters = false,
            };

            List<PersonAddRequest> peopleToAdd= new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3,
                personAddRequest4,
                personAddRequest5,
            };

            List<PersonResponse>? listOfPeopleAdded = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in peopleToAdd)
            {
                listOfPeopleAdded.Add(_peopleService.AddPerson(personAddRequest));
            }

            return listOfPeopleAdded;
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
            List<CountryResponse?> countriesAdded = AddingCountries();
            List<PersonResponse> listOfPeopleAdded = AddingPeople(countriesAdded);
            List<PersonResponse> listOfPeople = _peopleService.GetPeople();

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
        public void GetFilteredPeople_EmptySearch()
        {
            List<CountryResponse?> countriesAdded = AddingCountries();
            List<PersonResponse> listOfPeopleAdded = AddingPeople(countriesAdded);
            List<PersonResponse> listOfPeople = _peopleService.GetFilteredPeople(nameof(Person.PersonName), "");

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
        public void GetFilteredPeople_SearchByPersonName()
        {
            List<CountryResponse?> countriesAdded = AddingCountries();
            List<PersonResponse> listOfPeopleAdded = AddingPeople(countriesAdded);
            List<PersonResponse>? listOfPeople = _peopleService.GetFilteredPeople(nameof(Person.Gender), "Other");

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
        public void GetSortedPeople_EmptySearch()
        {
            List<CountryResponse?> countriesAdded = AddingCountries();
            List<PersonResponse> listOfPeopleAdded = AddingPeople(countriesAdded);

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
    }
}
