using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ContactsManager.Services
{
    public class CountriesService : ICountriesService
    {
        private ApplicationDbContext _peopleDbContext;

        public CountriesService(ApplicationDbContext peopleDbContext)
        {
            _peopleDbContext = peopleDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest is null) throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName is null) throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if(await _peopleDbContext.Countries.Where(country => country.CountryName == countryAddRequest.CountryName.Trim()).CountAsync() > 0) throw new ArgumentException("The country name given is already in the list.");

            Country newCountry = countryAddRequest.ToCountry();

            newCountry.CountryId = Guid.NewGuid();
            _peopleDbContext.Countries.Add(newCountry);
            await _peopleDbContext.SaveChangesAsync();

            return newCountry.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetCountries()
        {
            return await _peopleDbContext.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null) return null;

            Country? countryResponse = await _peopleDbContext.Countries.FirstOrDefaultAsync(country => country.CountryId.Equals(countryId));

            if (countryResponse is null) return null;

            return countryResponse.ToCountryResponse();
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
                            CountryAddRequest countryAddRequest = new CountryAddRequest()
                            {
                                CountryName = cellValue,
                            };

                            await AddCountry(countryAddRequest);
                            countriesAdded++;
                        }
                    } catch(Exception) {}
                }
            }

            return countriesAdded;
        }
    }
}
