using Microsoft.Extensions.Logging;
using Serilog;
using CsvHelper;
using OfficeOpenXml;
using SerilogTimings;
using System.Globalization;
using CsvHelper.Configuration;
using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class PeopleGetterService : IPeopleGetterService
    {
        private readonly IPeopleRepository _peopleRepository;
        private readonly ILogger<PeopleAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PeopleGetterService(IPeopleRepository peopleRepository, ILogger<PeopleAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _peopleRepository = peopleRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<List<PersonResponse>> GetPeople()
        {
            _logger.LogInformation("GetPeople of PeopleService.");
            return (await _peopleRepository.GetPeople()).Select(p => p.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            if (personId is null) return null;

               Person? person = await _peopleRepository.GetPersonByPersonId(personId.Value);

            if (person is null) return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>?> GetFilteredPeople(string searchBy, string? query)
        {
            _logger.LogInformation("GetFilteredPeople of PeopleService.");
            List<Person>? matchingPeople = new List<Person>();

            using (Operation.Time("Time for GetFilteredPeople get data from DB."))
            {
                matchingPeople = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _peopleRepository.GetFilteredPeople((person =>
                            (!string.IsNullOrEmpty(person.PersonName))
                                ? person.PersonName.Contains(query ?? "")
                                : true
                        )),

                    nameof(PersonResponse.PersonEmail) =>
                        await _peopleRepository.GetFilteredPeople(person =>
                            (!string.IsNullOrEmpty(person.PersonEmail))
                                ? person.PersonEmail.Contains(query ?? "")
                                : true
                        ),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _peopleRepository.GetFilteredPeople(person =>
                            (!string.IsNullOrEmpty(person.DateOfBirth.ToString()))
                                ? person.DateOfBirth!.Value.ToString().Contains(query ?? "")
                                : true
                        ),

                    nameof(PersonResponse.Gender) =>
                        await _peopleRepository.GetFilteredPeople(person =>
                            (!string.IsNullOrEmpty(person.Gender))
                                ? person.Gender.Equals(query)
                                : true
                        ),

                    nameof(PersonResponse.CountryName) =>
                        await _peopleRepository.GetFilteredPeople(person =>
                            (!string.IsNullOrEmpty(person.Country!.CountryName))
                                ? person.Country.CountryName.Contains(query ?? "")
                                : true
                        ),

                    nameof(PersonResponse.Address) =>
                        await _peopleRepository.GetFilteredPeople(person =>
                            (!string.IsNullOrEmpty(person.Address))
                                ? person.Address.Contains(query ?? "")
                                : true
                        ),

                    _ =>
                        await _peopleRepository.GetPeople()
                };
            }

            _diagnosticContext.Set("People", matchingPeople ?? new List<Person>());

            return matchingPeople?.Select(p => p.ToPersonResponse()).ToList();
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
