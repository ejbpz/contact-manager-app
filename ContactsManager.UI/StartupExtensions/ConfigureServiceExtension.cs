using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ContactsManager.Models;
using ContactsManager.Services;
using ContactsManager.Repositories;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;
using ContactsManager.Filters.ActionFilters;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Mvc;

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

                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
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

            // Authorization
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("NotAuthenticated", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 3;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>()
                .AddDefaultTokenProviders();

            return services;
        } 
    }
}
