using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TodoAppFunction
{
    public static class TodoTrigger
    {
        [FunctionName("TodoTrigger")]
        public static async Task Run(
            [CosmosDBTrigger(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODO,
                Connection = DBConfig.CONNECTION,
                CreateLeaseContainerIfNotExists = true)]IReadOnlyList<Model.Todo> todos,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODOHISTORY,
                Connection = DBConfig.CONNECTION)]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# CosmosDB trigger function processed a TodoModifiedTrigger request.");
            if (todos != null && todos.Count > 0)
            {
                log.LogInformation("Documents modified " + todos.Count);
                log.LogInformation("First document Id " + todos[0].Id);
                foreach (Model.Todo todo in todos)
                {
                    Model.TodoHistory todoHistory = Model.TodoHistory.CreateFrom(todo);
                    await documentsOut.AddAsync(todoHistory);
                    log.LogWarning($"captured as a history: {JsonConvert.SerializeObject(todoHistory)}");
                }
            }
        }
    }
}
