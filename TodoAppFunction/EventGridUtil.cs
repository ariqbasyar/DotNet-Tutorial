using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoAppFunction
{
    public static class EventGridUtil
    {
        public static readonly string KEY =
            Environment.GetEnvironmentVariable("KeyEvg");
        public static readonly string ENDPOINT =
            Environment.GetEnvironmentVariable("EndpointEvg");

        public static async Task SendDataToEventGrid(
            Model.Todo todo,
            string subject,
            string eventType = "Model.Todo")
        {
            var eventData = new EventGridEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Subject = subject,
                EventType = eventType,
                DataVersion = "1.0",
                EventTime = DateTime.Now,
                Data = JsonConvert.SerializeObject(todo)
            };

            var topicHostName = new Uri(ENDPOINT).Host;
            var cred = new TopicCredentials(KEY);
            var eventGridClient = new EventGridClient(cred);

            await eventGridClient.PublishEventsAsync(topicHostName,
                new List<EventGridEvent> { eventData });
        }
    }
}
