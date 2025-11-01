using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;

namespace ContactsManager.Services
{
    public class PeopleDeleterService : IPeopleDeleterService
    {
        private readonly IPeopleRepository _peopleRepository;

        public PeopleDeleterService(IPeopleRepository peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId is null || personId.Value == Guid.Empty) return false;

            return await _peopleRepository.DeletePersonByPersonId(personId.Value);
        }
    }
}
