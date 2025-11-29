using Microsoft.Extensions.Logging;
using Serilog;
using OfficeOpenXml;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class PeopleGetterServiceChild : PeopleGetterService
    {
        private readonly IPeopleGetterService _peopleGetterService;

        public PeopleGetterServiceChild(IPeopleRepository peopleRepository, 
                                        ILogger<PeopleGetterService> logger, 
                                        IDiagnosticContext diagnosticContext,
                                        IPeopleGetterService peopleGetterService) 
            : base(peopleRepository, logger, diagnosticContext)
        {
            _peopleGetterService = peopleGetterService;
        }

        public override async Task<MemoryStream> GetPeopleExcel()
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
    }
}
