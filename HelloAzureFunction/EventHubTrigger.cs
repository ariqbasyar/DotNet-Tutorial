using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HelloAzureFunction
{
    public class EventHubTrigger
    {
        private readonly ILogger<EventHubTrigger> _logger;

        public EventHubTrigger(ILogger<EventHubTrigger> logger)
        {
            _logger = logger;
        }

        [Function(nameof(EventHubTrigger))]
        public void Run([EventHubTrigger("samples-workitems", Connection = "")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                _logger.LogInformation("Event Body: {body}", @event.Body);
                _logger.LogInformation("Event Content-Type: {contentType}", @event.ContentType);
            }
        }
    }
}
