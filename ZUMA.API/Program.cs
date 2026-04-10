using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ZUMA.API.Configuration;
using ZUMA.API.Middleware;
using ZUMA.SharedKernel.Configuration;

// Vyčištění claim mapování hned na začátku
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

#region Services Configuration

builder.Services.AddControllers();

// JWT Autentizace - Produkční nastavení
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            ClockSkew = TimeSpan.Zero // Přísná kontrola expirace bez 5min tolerance
        };
    });

builder.Services.AddAuthorization();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

#endregion

#region Infrastructure (RabbitMQ, DB, DI)

// MassTransit & RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ__USERNAME"] ?? "guest");
            h.Password(builder.Configuration["RABBITMQ__PASSWORD"] ?? "guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

// Externí DI kontejnery
DIContainer.ConfigureBaseServices(builder.Services, builder.Configuration);
ApiDiContainer.ConfigureServices(builder.Services);

// Health Checks
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString!, name: "PostgreSQL Database")
    .AddTcpHealthCheck(opt => opt.AddHost("communication-service", 8081), name: "Communication Service")
    .AddTcpHealthCheck(opt => opt.AddHost("customer-service", 8082), name: "Customer Service");

#endregion

#region Swagger

builder.Services.AddSwaggerGen(options =>
{
    var apiInfo = new OpenApiInfo
    {
        Title = "ZUMA API",
        Description = "REST API pro správu uživatelů",
        Contact = new OpenApiContact { Name = "ZUMA Team", Email = "tomas.dohnal46@seznam.cz" },
        License = new OpenApiLicense { Name = "MIT" }
    };

    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1.0" });
    options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2.0", Description = apiInfo.Description + " (v2 - s stránkováním)" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // Vráceno na Http pro pohodlí ve Swaggeru
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Vložte pouze JWT token."
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "ZUMA.API.xml");
    if (File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);
});

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:50444")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ZUMA API v1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "ZUMA API v2.0");
    options.RoutePrefix = string.Empty;
});

// Pořadí je důležité!
app.UseHttpsRedirection();
app.UseCors();

// Auth musí být PŘED Controllers, ale PO Cors
app.UseAuthentication();
app.UseAuthorization();

// Pokud chceš logovat i auth/unauth přístupy, dej to pod Authentication
app.UseRequestResponseLogging();

app.MapControllers();

app.MapHealthChecks("/api/system-status", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            OverallStatus = report.Status.ToString(),
            Services = report.Entries.Select(e => new { Name = e.Key, Status = e.Value.Status.ToString() })
        };
        await System.Text.Json.JsonSerializer.SerializeAsync(context.Response.Body, response);
    }
});

#endregion

app.Run();