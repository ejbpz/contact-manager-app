using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services;

namespace ContactsManager.Test
{
    public class PeopleServiceTest
    {
        private readonly IPeopleService _peopleService;

        public PeopleServiceTest()
        {
            _peopleService = new PeopleService();
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
                PersonEmail = "edubpz02@gmail.com",
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
    }
}
