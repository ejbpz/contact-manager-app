using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.Services
{
    public class PeopleService : IPeopleService
    {
        private List<PersonResponse> _peopleList;

        public PeopleService()
        {
            _peopleList = new List<PersonResponse>();
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null) throw new ArgumentNullException(nameof(personAddRequest));

            if (string.IsNullOrEmpty(personAddRequest.PersonName)) throw new ArgumentException("PersonName cannot be null.");

            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            PersonResponse personResponse = person.ToPersonResponse();

            _peopleList.Add(personResponse);

            return personResponse;
        }

        public List<PersonResponse> GetPeople()
        {
            return _peopleList;
        }
    }
}
