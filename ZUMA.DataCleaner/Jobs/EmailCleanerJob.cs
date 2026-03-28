using Quartz;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.DataCleaner.Services;

namespace ZUMA.DataCleaner.Jobs
{
    [DisallowConcurrentExecution]
    public class EmailCleanerJob : IJob
    {
        private readonly ILogger<EmailCleanerJob> _logger;
        private readonly DataCleanerService<EmailEntity> _cleanerService;

        public EmailCleanerJob(ILogger<EmailCleanerJob> logger, DataCleanerService<EmailEntity> cleanerService)
        {
            _logger = logger;
            _cleanerService = cleanerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("EmailCleanerJob started. JobKey: {JobKey}", context.JobDetail.Key);

            try
            {
                await _cleanerService.CleanDataAsync(context.CancellationToken);

                _logger.LogInformation("EmailCleanerJob completed successfully at {Time}", DateTimeOffset.UtcNow);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("EmailCleanerJob was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during EmailCleanerJob execution. Target: EmailEntity");

                throw new JobExecutionException(ex) { RefireImmediately = false };
            }
        }
    }
}