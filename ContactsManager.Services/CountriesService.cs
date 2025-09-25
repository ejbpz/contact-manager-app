using ContactsManager.Models;
using ContactsManager.Repository;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ContactsManager.Services
{
    public class CountriesService : ICountriesService
    {
        private ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
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

        public async Task<List<CountryResponse>> GetCountries()
        {
            return (await _countriesRepository.GetCountries()).Select(c => c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId is null || !countryId.HasValue) return null;

            var countryResponse = await _countriesRepository.GetCountryByCountryId(countryId.Value);

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
