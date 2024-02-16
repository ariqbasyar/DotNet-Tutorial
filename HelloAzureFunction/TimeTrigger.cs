using System;
using System.Diagnostics.Metrics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HelloAzureFunction
{
    public class TimeTrigger
    {
        private readonly ILogger _logger;
        private static int counter = 0;

        public TimeTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimeTrigger>();
        }

        [Function("TimeTrigger")]
        public void Run([TimerTrigger("00:00:02", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogWarning($"Counter ke-: {counter}");
            counter++;
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
