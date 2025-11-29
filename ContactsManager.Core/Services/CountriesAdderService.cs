using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Services
{
    public class CountriesAdderService : ICountriesAdderService
    {
        private ICountriesRepository _countriesRepository;

        public CountriesAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest is null) throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName is null) throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) is not null)  throw new ArgumentException("The country name given is already in the list.");

            Country newCountry = countryAddRequest.ToCountry();
            newCountry.CountryId = Guid.NewGuid();

            await _countriesRepository.AddCountry(newCountry);
            return newCountry.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesAdded = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

                    try
                    {
                        if(!string.IsNullOrEmpty(cellValue))
                        {
                            if(await _countriesRepository.GetCountryByCountryName(cellValue) is null)
                            {
                                Country country = new Country()
                                {
                                    CountryName = cellValue,
                                };

                                await _countriesRepository.AddCountry(country);
                                countriesAdded++;
                            }
                        }
                    } catch(Exception) {}
                }
            }

            return countriesAdded;
        }
    }
}
