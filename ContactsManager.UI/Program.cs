using Serilog;
using ContactsManager;
using ContactsManager.Middlewares;

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

// Middleware pipeline

// Error screen
if (builder.Environment.IsDevelopment()) 
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Rotativa to PDFs
if (!builder.Environment.IsEnvironment("Test")) 
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseEndpoints(endpoint =>
{
    endpoint.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Admin}/{action=Index}"
    );

    endpoint.MapControllerRoute(
        name: "default", 
        pattern: "{controller}/{action}"
    );
});

app.Run();

public partial class Program { }