using Quartz;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.DataCleaner.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddQuartz(q =>
{
    // vytvoříme unikátní klíč pro náš Job
    var jobKey = new JobKey("DailyDataCleanerJob");

    // Zaregistrujeme Job
    q.AddJob<UserCleanerJob>(opts => opts.WithIdentity(jobKey));

    // Vytvoříme Trigger (Schedule)
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DailyDataCleanerTrigger")
        // Cron: 0 (sekunda) 0 (minuta) 0 (hodina) -> Půlnoc každý den
        .WithCronSchedule("0 0 0 * * ?")
        .WithDescription("Pravidelné čištění dat každý den o půlnoci"));
});

var host = builder.Build();
host.Run();
