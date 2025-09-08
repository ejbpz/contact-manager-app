using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services.Helpers;

namespace ContactsManager.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly PeopleDbContext _peopleDbContext;
        private readonly ICountriesService _countriesService;

        public PeopleService(PeopleDbContext peopleDbContext, ICountriesService countriesService)
        {
            _peopleDbContext = peopleDbContext;
            _countriesService = countriesService;
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

            _peopleDbContext.sp_AddPerson(person);

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetPeople()
        {
            return _peopleDbContext.sp_GetPeople().Select(p => ConvertPersonToPersonResponse(p)).ToList();
        }

        public PersonResponse? GetPersonByPersonId(Guid? personId)
        {
            if (personId is null) return null;

               Person? person = _peopleDbContext.People.FirstOrDefault(person => person.PersonId == personId);

            if (person is null) return null;
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPeople(string searchBy, string? query)
        {
            List<PersonResponse> allPeople = GetPeople();
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(query)) return allPeople;
            
            List<PersonResponse> matchingPeople = new List<PersonResponse>();

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPeople = allPeople.Where(person => 
                        (!string.IsNullOrEmpty(person.PersonName)) 
                            ? person.PersonName.Contains(query, StringComparison.OrdinalIgnoreCase)
                            : true
                    ).ToList();
                    break;
                case nameof(PersonResponse.PersonEmail):
                    matchingPeople = allPeople.Where(person =>
                        (!string.IsNullOrEmpty(person.PersonEmail))
                            ? person.PersonEmail.Contains(query, StringComparison.OrdinalIgnoreCase)
                            : true
                    ).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPeople = allPeople.Where(person =>
                        (!string.IsNullOrEmpty(person.DateOfBirth.ToString()))
                            ? person.DateOfBirth!.Value.ToString("dd MMMM yyyy").Contains(query, StringComparison.OrdinalIgnoreCase)
                            : true
                    ).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPeople = allPeople.Where(person =>
                        (!string.IsNullOrEmpty(person.Gender))
                            ? person.Gender.Equals(query, StringComparison.OrdinalIgnoreCase)
                            : true
                    ).ToList();
                    break;
                case nameof(PersonResponse.CountryName):
                    matchingPeople = allPeople.Where(person =>
                        (!string.IsNullOrEmpty(person.CountryName))
                            ? person.CountryName.Contains(query, StringComparison.OrdinalIgnoreCase)
                            : true
                    ).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPeople = allPeople.Where(person =>
                        (!string.IsNullOrEmpty(person.Address))
                            ? person.Address.Contains(query, StringComparison.OrdinalIgnoreCase)
                            : true
                    ).ToList();
                    break;
                default:
                    matchingPeople = allPeople;
                    break;
            }

            return matchingPeople;
        }

        public List<PersonResponse> GetSortedPeople(List<PersonResponse> allPeople, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy)) return allPeople;

            List<PersonResponse> sortedPeople = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonEmail), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.PersonEmail, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonEmail), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.PersonEmail, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.CountryName), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.IsReceivingNewsLetters), SortOrderOptions.Ascending) => allPeople.OrderBy(p => p.IsReceivingNewsLetters).ToList(),
                (nameof(PersonResponse.IsReceivingNewsLetters), SortOrderOptions.Descending) => allPeople.OrderByDescending(p => p.IsReceivingNewsLetters).ToList(),

                _ => allPeople
            };

            return sortedPeople;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest is null) throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? personToUpdate = _peopleDbContext.People.FirstOrDefault(p => p.PersonId == personUpdateRequest.PersonId);

            if (personToUpdate is null) throw new ArgumentException("Given person Id doesn't exist.");

            personToUpdate.PersonName = personUpdateRequest.PersonName;
            personToUpdate.PersonEmail = personUpdateRequest.PersonEmail;
            personToUpdate.DateOfBirth = personUpdateRequest.DateOfBirth;
            personToUpdate.Gender = personUpdateRequest.Gender.ToString();
            personToUpdate.CountryId = personUpdateRequest.CountryId;
            personToUpdate.Address = personUpdateRequest.Address;
            personToUpdate.IsReceivingNewsLetters = personUpdateRequest.IsReceivingNewsLetters;
            
            _peopleDbContext.SaveChanges();

            return ConvertPersonToPersonResponse(personToUpdate);
        }

        public bool DeletePerson(Guid? personId)
        {
            if (personId is null) return false;

            Person? person = _peopleDbContext.People.FirstOrDefault(p => p.PersonId == personId);

            if (person is null) return false;

            _peopleDbContext.People.Remove(person);
            _peopleDbContext.SaveChanges();

            return true;
        }
    }
}
