using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using ContactsManager.Models;
using ContactsManager.Services;
using ContactsManager.Repositories;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.Filters.ActionFilters;

namespace ContactsManager
{
    public static class ConfigureServiceExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IWebHostEnvironment environment, ConfigurationManager configuration)
        {
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<PeopleListActionFilter>(5);
                //var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                options.Filters.Add(new ResponseHeaderFilterFactory("my-global-key", "my-global-value", 2));
            });

            // Services
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });


            services.AddScoped<ICountriesRepository, CountriesRepository>();
            
            services.AddScoped<IPeopleRepository, PeopleRepository>();

            services.AddScoped<ICountriesAdderService, CountriesAdderService>();
            services.AddScoped<ICountriesGetterService, CountriesGetterService>();

            services.AddScoped<PeopleGetterService, PeopleGetterService>();
            services.AddScoped<IPeopleGetterService, PeopleGetterServiceWithFewExcelFields>();
            services.AddScoped<IPeopleAdderService, PeopleAdderService>();
            services.AddScoped<IPeopleDeleterService, PeopleDeleterService>();
            services.AddScoped<IPeopleUpdaterService, PeopleUpdaterService>();
            services.AddScoped<IPeopleSorterService, PeopleSorterService>();

            services.AddTransient<PeopleListActionFilter>();
            services.AddTransient<ResponseHeaderActionFilter>();

            // DbContext
            if(!environment.IsEnvironment("Test"))
            {
                services.AddDbContext<ApplicationDbContext>(
                    options => {
                        options.UseSqlServer(configuration.GetConnectionString("PeopleDBConnection"));
                    });
            }

            return services;
        } 
    }
}
