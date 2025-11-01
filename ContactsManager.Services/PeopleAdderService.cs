using Microsoft.Extensions.Logging;
using Serilog;
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
        private readonly ILogger<PeopleAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PeopleAdderService(IPeopleRepository peopleRepository, ILogger<PeopleAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _peopleRepository = peopleRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
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
