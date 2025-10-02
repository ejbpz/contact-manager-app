using ContactsManager.Models;
using ContactsManager.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ContactsManager.Repositories
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly ApplicationDbContext _context;

        public PeopleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();
            
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _context.RemoveRange(_context.People.Where(p => p.PersonId == personId));
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Person>?> GetFilteredPeople(Expression<Func<Person, bool>> predicate)
        {
            return await _context.People
                .Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<List<Person>> GetPeople()
        {
            return await _context.People
                .Include("Country")
                .ToListAsync();
        }

        public Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return _context.People
                .Include("Country")
                .FirstOrDefaultAsync(p => p.PersonId == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? personToUpdate = await GetPersonByPersonId(person.PersonId);

            if (personToUpdate is null) return person;

            personToUpdate.PersonName = person.PersonName ?? personToUpdate.PersonName;
            personToUpdate.PersonEmail = person.PersonEmail ?? personToUpdate.PersonEmail;
            personToUpdate.DateOfBirth = person.DateOfBirth ?? personToUpdate.DateOfBirth;
            personToUpdate.Gender = person.Gender?.ToString() ?? personToUpdate.Gender;
            personToUpdate.CountryId = person.CountryId ?? personToUpdate.CountryId;
            personToUpdate.Address = person.Address ?? personToUpdate.Address;
            personToUpdate.IsReceivingNewsLetters = person.IsReceivingNewsLetters;
            personToUpdate.TIN = person.TIN ?? personToUpdate.TIN;

            await _context.SaveChangesAsync();
            return personToUpdate;
        }
    }
}
