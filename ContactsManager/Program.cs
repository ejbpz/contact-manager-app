using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using ContactsManager.Models;
using ContactsManager.Services;
using ContactsManager.Repositories;
using ContactsManager.ServiceContracts;
using ContactsManager.RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

// Logging - Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

// Views to Controller
builder.Services.AddControllersWithViews();

// Services
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPeopleService, PeopleService>();

// DbContext
if(!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(
        options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("PeopleDBConnection"));
        });
}

var app = builder.Build();

// Error screen
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

// Rotativa to PDFs
if(!builder.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.Run();

public partial class Program { }