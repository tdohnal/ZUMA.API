using Asp.Versioning;
using Microsoft.OpenApi;
using System.Diagnostics;
using ZUMA.API.Configuration;
using ZUMA.API.Middleware;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.BussinessLogic.Plugins;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ZUMA API",
        Version = "v1.0",
        Description = "REST API pro správu uživatelů",
        Contact = new OpenApiContact
        {
            Name = "ZUMA Team",
            Email = "tomas.dohnal46@seznam.cz",
            Url = new Uri("https://github.com/tdohnal/ZUMA.API")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "ZUMA API",
        Version = "v2.0",
        Description = "REST API pro správu uživatelů (v2 - s stránkováním)",
        Contact = new OpenApiContact
        {
            Name = "ZUMA Team",
            Email = "support@zuma.cz",
            Url = new Uri("https://github.com/tdohnal/ZUMA.API")
        }
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "ZUMA.API.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }
});

builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

DIContainer.ConfigureServices(builder.Services, builder.Configuration);
ApiDiContainer.ConfigureServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ZUMA API v1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "ZUMA API v2.0");
        options.RoutePrefix = string.Empty;
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
    });

    app.MapOpenApi();
    ConsoleManager.Show();

    var launchUrl = "http://localhost:5044";
    Task.Run(() =>
    {
        System.Threading.Thread.Sleep(1000);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = launchUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Nelze automaticky otevřít prohlížeč: {ex.Message}");
        }
    });
}

app.UseRequestResponseLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
