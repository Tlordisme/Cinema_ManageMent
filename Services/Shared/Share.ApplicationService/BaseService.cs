using Microsoft.Extensions.Logging;

namespace Share.ApplicationService
{
    public abstract class BaseService
    {
        protected readonly ILogger<BaseService> _logger;

        protected BaseService(ILogger<BaseService> logger)
        {
            _logger = logger;
        }

        protected void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        protected void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        protected void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
