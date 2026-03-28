using Quartz;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.DataCleaner.Services;

namespace ZUMA.DataCleaner.Jobs
{
    [DisallowConcurrentExecution]
    public class RegistrationCleanerJob : IJob
    {
        private readonly ILogger<RegistrationCleanerJob> _logger;
        private readonly DataCleanerService<RegistrationEntity> _cleanerService;

        public RegistrationCleanerJob(ILogger<RegistrationCleanerJob> logger, DataCleanerService<RegistrationEntity> cleanerService)
        {
            _logger = logger;
            _cleanerService = cleanerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("RegistrationCleanerJob started. JobKey: {JobKey}", context.JobDetail.Key);

            try
            {
                await _cleanerService.CleanDataAsync(context.CancellationToken);

                _logger.LogInformation("RegistrationCleanerJob completed successfully at {Time}", DateTimeOffset.UtcNow);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("RegistrationCleanerJob was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during RegistrationCleanerJob execution. Target: RegistrationEntity");

                throw new JobExecutionException(ex) { RefireImmediately = false };
            }
        }
    }
}