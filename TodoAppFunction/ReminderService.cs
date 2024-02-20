// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAppFunction
{
    public static class ReminderService
    {
        [FunctionName("ReminderServiceConsumer")]
        public static async Task Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            ILogger log)
        {
            var strData = eventGridEvent.Data.ToString();
            log.LogInformation("C# EventGrid trigger function processed a Reminder request.");
            log.LogWarning($"subject: {eventGridEvent.Subject}");
            log.LogWarning($"eventType: {eventGridEvent.EventType}");
            log.LogWarning($"data: {strData}");

            var evgTodo = JsonConvert.DeserializeObject<Model.Todo>(strData);

            var client = new CosmosClient(DBConfig.CONNECTIONSTRING);
            Container container = client.GetDatabase(DBConfig.DATABASE)
                .GetContainer(DBConfig.CONTAINERREMINDER);
            var foundReminder =
                container.GetItemLinqQueryable<Model.Reminder>(true)
                    .Where(p => p.TodoId == evgTodo.Id)
                    .AsEnumerable()
                    .FirstOrDefault();

            if (foundReminder == null)
            {
                log.LogWarning("New Reminder");
                foundReminder = Model.Reminder.CreateFrom(evgTodo);
                var partition = new PartitionKey(foundReminder.TodoId);
                await container.CreateItemAsync(foundReminder,partition);
                log.LogWarning(JsonConvert.SerializeObject(foundReminder));
            }
            else
            {
                log.LogWarning("Edit Reminder");
                log.LogWarning(JsonConvert.SerializeObject(foundReminder));
                log.LogWarning("to");
                foundReminder.IsCompleted = evgTodo.IsComplete ?? false;
                var partition = new PartitionKey(foundReminder.TodoId);
                await container.ReplaceItemAsync(foundReminder,foundReminder.Id,
                    partition);
                log.LogWarning(JsonConvert.SerializeObject(foundReminder));
            }
        }
    }
}
