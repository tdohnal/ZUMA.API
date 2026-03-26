using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using ZUMA.API.Configuration;
using ZUMA.API.Middleware;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Token vyprší přesně v daný čas (bez 5min rezervy)
        };
    });

builder.Services.AddAuthorization();

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

builder.Services.AddSwaggerGen(options =>
{
    // 1. Definice v1 dokumentace
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

    // 2. Definice v2 dokumentace
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

    // --- ZAČÁTEK KONFIGURACE PRO JWT ---

    // 3. Definice bezpečnostního schématu (To vytvoří tlačítko Authorize)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Zadejte pouze samotný JWT token. Systém automaticky doplní 'Bearer ' před něj."
    });

    // 4. Globální požadavek na zabezpečení (Přidá ikonku zámku ke všem endpointům)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // --- KONEC KONFIGURACE PRO JWT ---

    // 5. Načtení XML komentářů (pokud existují)
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "ZUMA.API.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }
});
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

#region Swagger

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
}

#endregion

app.UseRequestResponseLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.Run();

