using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ICountriesService, CountriesService>();
builder.Services.AddSingleton<IPeopleService, PeopleService>();

builder.Services.AddDbContext<PeopleDbContext>(
    options => {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PeopleDBConnection"));
    });

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
