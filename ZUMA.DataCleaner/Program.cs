using MassTransit;
using Quartz;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.DataCleaner.Configuration;
using ZUMA.DataCleaner.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

//current DI
builder.Services.ConfigureServices();

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

builder.Services.AddQuartz(q =>
{
    // 1. USER CLEANER
    var userJobKey = new JobKey("UserCleanerJob", "DailyGroup");
    q.AddJob<UserCleanerJob>(opts => opts.WithIdentity(userJobKey));
    q.AddTrigger(opts => opts
        .ForJob(userJobKey)
        .WithIdentity("UserCleanerTrigger")
        .WithCronSchedule("0 0 0 * * ?")
        .WithDescription("Cleans users at midnight"));

    // 2. REGISTRATION CLEANER
    var regJobKey = new JobKey("RegistrationCleanerJob", "DailyGroup");
    q.AddJob<RegistrationCleanerJob>(opts => opts.WithIdentity(regJobKey));
    q.AddTrigger(opts => opts
        .ForJob(regJobKey)
        .WithIdentity("RegistrationCleanerTrigger")
        .WithCronSchedule("0 0 0 * * ?")
        .WithDescription("Cleans registrations at midnight"));

    // 3. EMAIL CLEANER
    var emailJobKey = new JobKey("EmailCleanerJob", "DailyGroup");
    q.AddJob<EmailCleanerJob>(opts => opts.WithIdentity(emailJobKey));
    q.AddTrigger(opts => opts
        .ForJob(emailJobKey)
        .WithIdentity("EmailCleanerTrigger")
        .WithCronSchedule("0 0 0 * * ?")
        .WithDescription("Cleans emails at midnight"));
});

// Nezapomeň, že HostedService musí být zaregistrován až POTÉ, co zkonfiguruješ Quartz
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
var host = builder.Build();
host.Run();
