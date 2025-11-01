using Microsoft.Extensions.Logging;
using ContactsManager.Models;
using ContactsManager.Exceptions;
using ContactsManager.Services.Helpers;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class PeopleUpdaterService : IPeopleUpdaterService
    {
        private readonly IPeopleRepository _peopleRepository;
        private readonly ILogger<PeopleAdderService> _logger;
        private readonly IPeopleGetterService _peopleGetterService;

        public PeopleUpdaterService(IPeopleGetterService peopleGetterService, IPeopleRepository peopleRepository, ILogger<PeopleAdderService> logger)
        {
            _peopleRepository = peopleRepository;
            _logger = logger;
            _peopleGetterService = peopleGetterService;
        }

        public async Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest is null) throw new ArgumentNullException(nameof(personUpdateRequest));

            if (personUpdateRequest.PersonId == Guid.Empty || personUpdateRequest?.PersonId == null) throw new ArgumentException("The person Id is null.");

            ValidationHelper.ModelValidation(personUpdateRequest);


            Person? personToUpdate = (await _peopleGetterService.GetPersonByPersonId(personUpdateRequest.PersonId))?.ToPersonUpdateRequest().ToPerson();

            if (personToUpdate is null) throw new InvalidPersonIdException("Given person Id doesn't exist.");

            return (await _peopleRepository.UpdatePerson(personToUpdate, personUpdateRequest.ToPerson())).ToPersonResponse();
        }
    }
}
