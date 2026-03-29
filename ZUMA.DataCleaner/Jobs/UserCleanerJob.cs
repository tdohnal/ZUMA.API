using Quartz;
using ZUMA.DataCleaner.Services;

namespace ZUMA.DataCleaner.Jobs
{
    [DisallowConcurrentExecution]
    public class UserCleanerJob : IJob
    {
        private readonly ILogger<UserCleanerJob> _logger;
        private readonly DataCleanerService<UserEntity> _cleanerService;

        public UserCleanerJob(ILogger<UserCleanerJob> logger, DataCleanerService<UserEntity> cleanerService)
        {
            _logger = logger;
            _cleanerService = cleanerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("UserCleanerJob started. JobKey: {JobKey}", context.JobDetail.Key);

            try
            {
                await _cleanerService.CleanDataAsync(context.CancellationToken);

                _logger.LogInformation("UserCleanerJob completed successfully at {Time}", DateTimeOffset.UtcNow);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("UserCleanerJob was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during UserCleanerJob execution. Target: UserEntity");

                throw new JobExecutionException(ex) { RefireImmediately = false };
            }
        }
    }
}