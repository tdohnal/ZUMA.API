using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ZUMA.API.Configuration;
using ZUMA.API.Middleware;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);

#region Controllers & Auth

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

#endregion

#region RabbitMQ

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("zuma_admin");
            h.Password("moje_tajne_heslo_123");
        });
    });
});

#endregion

#region Swagger

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Zadejte pouze samotný JWT token. Systém automaticky doplní 'Bearer ' před něj."
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "ZUMA.API.xml");
    if (File.Exists(xmlFile))
        options.IncludeXmlComments(xmlFile);
});

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();

        policy.WithOrigins("http://localhost:50444")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

#endregion

#region API Versioning

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

#endregion

DIContainer.ConfigureServices(builder.Services, builder.Configuration);
ApiDiContainer.ConfigureServices(builder.Services);

var app = builder.Build();

#region EF Migration

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CustomerDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError("Migration Failed", ex);
    }
}

#endregion

#region Middleware & Pipeline

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
}

app.UseRequestResponseLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#endregion

app.Run();