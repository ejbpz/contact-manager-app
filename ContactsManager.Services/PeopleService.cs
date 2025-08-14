using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Services
{
    public class PeopleService : IPeopleService
    {
        private List<PersonResponse> _peopleList;
        private ICountriesService _countriesService;

        public PeopleService()
        {
            _peopleList = new List<PersonResponse>();
            _countriesService = new CountriesService();
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.CountryName = _countriesService.GetCountryByCountryId(personResponse.CountryId)?.CountryName;

            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null) throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            PersonResponse personResponse = ConvertPersonToPersonResponse(person);

            _peopleList.Add(personResponse);

            return personResponse;
        }

        public List<PersonResponse> GetPeople()
        {
            return _peopleList;
        }
    }
}
