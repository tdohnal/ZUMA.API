using Asp.Versioning;
using ZUMA.API.Configuration;
using ZUMA.API.Middleware;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.BussinessLogic.Plugins;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    ConsoleManager.Show();
}

app.UseRequestResponseLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
