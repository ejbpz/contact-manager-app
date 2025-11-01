using OfficeOpenXml;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class PeopleGetterServiceWithFewExcelFields : IPeopleGetterService
    {
        private readonly PeopleGetterService _peopleGetterService;

        public PeopleGetterServiceWithFewExcelFields(PeopleGetterService peopleGetterService)
        {
            _peopleGetterService = peopleGetterService;
        }

        public async Task<MemoryStream> GetPeopleExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("people-sheet");
                worksheet.Cells["A1"].Value = "Name";
                worksheet.Cells["B1"].Value = "Age";
                worksheet.Cells["C1"].Value = "Gender";

                using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#43A490"));
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                List<PersonResponse> people = await _peopleGetterService.GetPeople();

                foreach (PersonResponse person in people)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Age;
                    worksheet.Cells[row, 3].Value = person.Gender;
                    row++;
                }

                using (ExcelRange contentCells = worksheet.Cells[$"A2:C{row}"])
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

        public Task<List<PersonResponse>?> GetFilteredPeople(string searchBy, string? query)
        {
            return _peopleGetterService.GetFilteredPeople(searchBy, query);
        }

        public Task<List<PersonResponse>> GetPeople()
        {
            return _peopleGetterService.GetPeople();
        }

        public Task<MemoryStream> GetPeopleCSV()
        {
            return _peopleGetterService.GetPeopleCSV();
        }

        public Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            return _peopleGetterService.GetPersonByPersonId(personId);
        }
    }
}
