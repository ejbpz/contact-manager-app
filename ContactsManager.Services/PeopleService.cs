using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services.Helpers;

namespace ContactsManager.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly List<Person> _peopleList;
        private readonly ICountriesService _countriesService;

        public PeopleService(bool initialize = true)
        {
            _peopleList = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _peopleList = new List<Person>()
                {
                    new Person()
                    {
                        PersonId = Guid.Parse("A2EF3BEC-5E7F-40A7-AE9F-8ADFBA33E515"),
                        PersonName = "Alfy",
                        PersonEmail = "alukesch0@networksolutions.com",
                        DateOfBirth = new DateTime(2022, 10, 16),
                        Gender = "Male",
                        CountryId = Guid.Parse("6E3BE723-13E2-4068-B392-D9353ED41F6D"),
                        Address = "681 Sutherland Road",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("F87D9A0D-DC9B-4751-9D0A-274BD26C1ACB"),
                        PersonName = "Kevyb",
                        PersonEmail = "kcraft1@dagondesign.com",
                        DateOfBirth = new DateTime(2010, 3, 15),
                        Gender = "Female",
                        CountryId = Guid.Parse("13C250B3-2AF6-40A9-9616-6E5ABF1EA2A9"),
                        Address = "0 Springs Hill",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("254D8111-BF36-422C-A601-910D8CD3B7BD"),
                        PersonName = "Sayre",
                        PersonEmail = "sespinha2@auda.org.au",
                        DateOfBirth = new DateTime(2004, 3, 6),
                        Gender = "Male",
                        CountryId = Guid.Parse("D67524D3-BF47-4F48-AEF1-E20BBE3F0443"),
                        Address = "677 Nelson Junction",
                        IsReceivingNewsLetters = true,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("F530DD6D-175C-4C1C-8574-A2BBBAE1FC2D"),
                        PersonName = "Britt",
                        PersonEmail = "bferrolli3@tiny.cc",
                        DateOfBirth = new DateTime(1992, 11, 25),
                        Gender = "Male",
                        CountryId = Guid.Parse("969874C5-A77D-4158-B033-605FF940D04E"),
                        Address = "135 Clove Crossing",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("96D243CA-E71A-464C-8F7A-89D81DC60C2B"),
                        PersonName = "Allyn",
                        PersonEmail = "agarlee4@yolasite.com",
                        DateOfBirth = new DateTime(1987, 4, 15),
                        Gender = "Female",
                        CountryId = Guid.Parse("969874C5-A77D-4158-B033-605FF940D04E"),
                        Address = "9 Dakota Place",
                        IsReceivingNewsLetters = true,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("34DF3927-B48D-46D0-B515-58429145006F"),
                        PersonName = "Demetris",
                        PersonEmail = "dslimm5@miitbeian.gov.cn",
                        DateOfBirth = new DateTime(1987, 4, 15),
                        Gender = "Male",
                        CountryId = Guid.Parse("D67524D3-BF47-4F48-AEF1-E20BBE3F0443"),
                        Address = "53019 Daystar Plaza",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("0EAE4FE4-E774-4679-93D2-1DA948D25DCC"),
                        PersonName = "Hattie",
                        PersonEmail = "hmora6@google.ru",
                        DateOfBirth = new DateTime(2003, 2, 3),
                        Gender = "Female",
                        CountryId = Guid.Parse("FDDE1F72-2A86-4A39-9ECC-846D840B91B9"),
                        Address = "96 Welch Street",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("10B999CE-D7CD-48ED-A795-4BFE2B9C47D3"),
                        PersonName = "Lindsy",
                        PersonEmail = "lcrittal7@dmoz.org",
                        DateOfBirth = new DateTime(1990, 5, 17),
                        Gender = "Female",
                        CountryId = Guid.Parse("969874C5-A77D-4158-B033-605FF940D04E"),
                        Address = "802 Melody Alley",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("14C014A7-27EA-42B7-914D-5B6A8C827660"),
                        PersonName = "Jeffry",
                        PersonEmail = "jpodbury8@usatoday.com",
                        DateOfBirth = new DateTime(2002, 1, 6),
                        Gender = "Male",
                        CountryId = Guid.Parse("6E3BE723-13E2-4068-B392-D9353ED41F6D"),
                        Address = "09 Dwight Point",
                        IsReceivingNewsLetters = false,
                    },
                    new Person()
                    {
                        PersonId = Guid.Parse("CCF78AAD-0832-480E-8009-1190AE9F0445"),
                        PersonName = "Chaim",
                        PersonEmail = "cmatuschek9@microsoft.com",
                        DateOfBirth = new DateTime(1997, 10, 29),
                        Gender = "Male",
                        CountryId = Guid.Parse("FDDE1F72-2A86-4A39-9ECC-846D840B91B9"),
                        Address = "418 Veith Junction",
                        IsReceivingNewsLetters = true,
                    },
                };
            }
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

            _peopleList.Add(person);

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetPeople()
        {
            return _peopleList.Select(person => ConvertPersonToPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonId(Guid? personId)
        {
            if (personId is null) return null;

               Person? person = _peopleList.FirstOrDefault(person => person.PersonId == personId);

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
            if (string.IsNullOrEmpty(sortBy)) return _peopleList.Select(p => p.ToPersonResponse()).ToList();

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

            Person? personToUpdate = _peopleList.FirstOrDefault(p => p.PersonId == personUpdateRequest.PersonId);

            if (personToUpdate is null) throw new ArgumentException("Give person Id doesn't exist.");

            personToUpdate.PersonName = personUpdateRequest.PersonName;
            personToUpdate.PersonEmail = personUpdateRequest.PersonEmail;
            personToUpdate.DateOfBirth = personUpdateRequest.DateOfBirth;
            personToUpdate.Gender = personUpdateRequest.Gender.ToString();
            personToUpdate.CountryId = personUpdateRequest.CountryId;
            personToUpdate.Address = personUpdateRequest.Address;
            personToUpdate.IsReceivingNewsLetters = personUpdateRequest.IsReceivingNewsLetters;

            return ConvertPersonToPersonResponse(personToUpdate);
        }

        public bool DeletePerson(Guid? personId)
        {
            if (personId is null) return false;

            Person? person = _peopleList.FirstOrDefault(p => p.PersonId == personId);

            if (person is null) return false;

            return _peopleList.Remove(person);
        }
    }
}
