using ContactsManager.Models;
using ContactsManager.Services.Helpers;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class PeopleAdderService : IPeopleAdderService
    {
        private readonly IPeopleRepository _peopleRepository;

        public PeopleAdderService(IPeopleRepository peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null) throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            await _peopleRepository.AddPerson(person);

            return person.ToPersonResponse();
        }
    }
}
