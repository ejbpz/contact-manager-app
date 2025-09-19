using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services.Helpers;

namespace ContactsManager.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly ApplicationDbContext _peopleDbContext;

        public PeopleService(ApplicationDbContext peopleDbContext)
        {
            _peopleDbContext = peopleDbContext;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null) throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            _peopleDbContext.Add(person);
            await _peopleDbContext.SaveChangesAsync();

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetPeople()
        {
            return await _peopleDbContext.People.Include("Country").Select(p => p.ToPersonResponse()).ToListAsync();
        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            if (personId is null) return null;

               Person? person = await _peopleDbContext.People.FirstOrDefaultAsync(person => person.PersonId == personId);

            if (person is null) return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPeople(string searchBy, string? query)
        {
            List<PersonResponse> allPeople = await GetPeople();
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest is null) throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? personToUpdate = await _peopleDbContext.People.FirstOrDefaultAsync(p => p.PersonId == personUpdateRequest.PersonId);

            if (personToUpdate is null) throw new ArgumentException("Given person Id doesn't exist.");

            personToUpdate.PersonName = personUpdateRequest.PersonName;
            personToUpdate.PersonEmail = personUpdateRequest.PersonEmail;
            personToUpdate.DateOfBirth = personUpdateRequest.DateOfBirth;
            personToUpdate.Gender = personUpdateRequest.Gender.ToString();
            personToUpdate.CountryId = personUpdateRequest.CountryId;
            personToUpdate.Address = personUpdateRequest.Address;
            personToUpdate.IsReceivingNewsLetters = personUpdateRequest.IsReceivingNewsLetters;
            
            await _peopleDbContext.SaveChangesAsync();
            return personToUpdate.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId is null) return false;

            Person? person = await _peopleDbContext.People.FirstOrDefaultAsync(p => p.PersonId == personId);

            if (person is null) return false;

            _peopleDbContext.People.Remove(person);
            await _peopleDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<MemoryStream> GetPeopleCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            {
                CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

                CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

                csvWriter.WriteField(nameof(PersonResponse.PersonName));
                csvWriter.WriteField(nameof(PersonResponse.PersonEmail));
                csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
                csvWriter.WriteField(nameof(PersonResponse.Age));
                csvWriter.WriteField(nameof(PersonResponse.Gender));
                csvWriter.WriteField(nameof(PersonResponse.CountryName)); csvWriter.WriteField(nameof(PersonResponse.Address));
                csvWriter.WriteField(nameof(PersonResponse.IsReceivingNewsLetters));
                csvWriter.NextRecord();

                List<PersonResponse>? people = await GetPeople();

                foreach (PersonResponse person in people)
                {
                    csvWriter.WriteField(person.PersonName);
                    csvWriter.WriteField(person.PersonEmail);
                    csvWriter.WriteField(person.DateOfBirth.HasValue 
                        ? person.DateOfBirth.Value.ToString("dd/MM/yyyy") 
                        : "");
                    csvWriter.WriteField(person.Age);
                    csvWriter.WriteField(person.Gender);
                    csvWriter.WriteField(person.CountryName);
                    csvWriter.WriteField(person.Address);
                    csvWriter.WriteField(person.IsReceivingNewsLetters);
                    csvWriter.NextRecord();
                    await csvWriter.FlushAsync();
                }

            }

            MemoryStream newMemoryStream = new MemoryStream(memoryStream.ToArray());
            newMemoryStream.Position = 0;
            return newMemoryStream;
        }

        public async Task<MemoryStream> GetPeopleExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            //ExcelPackage.License.SetNonCommercialPersonal("");

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("people-sheet");
                worksheet.Cells["A1"].Value = "Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date of birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Address";
                worksheet.Cells["G1"].Value = "Country";
                worksheet.Cells["H1"].Value = "Receiving newsletters";

                using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#43A490"));
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                List<PersonResponse> people = await GetPeople();

                foreach(PersonResponse person in people)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.PersonEmail;
                    worksheet.Cells[row, 3].Value = person.DateOfBirth.HasValue 
                        ? person.DateOfBirth.Value.ToString("dd-MM-yyyy")
                        : "";
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Address;
                    worksheet.Cells[row, 7].Value = person.CountryName;
                    worksheet.Cells[row, 8].Value = person.IsReceivingNewsLetters;
                    row++;
                }

                using (ExcelRange contentCells = worksheet.Cells[$"A2:H{row}"])
                {
                    contentCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                worksheet.Cells[$"A1:H{row - 1}"].AutoFitColumns();
                worksheet.Cells[$"A1:H{row - 1}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells[$"A1:H{row - 1}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells[$"A1:H{row - 1}"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                worksheet.Cells[$"A1:H{row - 1}"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
