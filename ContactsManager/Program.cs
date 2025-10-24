using Serilog;
using ContactsManager;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Logging - Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

// Adding services
builder.Services.ConfigureServices(builder.Environment, builder.Configuration);

var app = builder.Build();

// Error screen
if (builder.Environment.IsDevelopment()) 
    app.UseDeveloperExceptionPage();

// Middleware pipeline
app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

// Rotativa to PDFs
if(!builder.Environment.IsEnvironment("Test")) 
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.Run();

public partial class Program { }