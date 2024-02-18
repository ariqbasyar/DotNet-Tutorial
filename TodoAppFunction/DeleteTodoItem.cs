using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace TodoAppFunction
{
    public static class DeleteTodoItem
    {
        [FunctionName("DeleteTodoItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Todo/{todoType}/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODO,
                Connection = DBConfig.CONNECTION,
                Id = "{id}",
                PartitionKey = "{todoType}")]Model.Todo todo,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a DeleteTodoItem request.");

                if (todo == null) throw new ArgumentNullException("User not found.");
                var client = new CosmosClient(DBConfig.CONNECTIONSTRING);
                Container cosmosContainer = client.GetDatabase(DBConfig.DATABASE)
                                                  .GetContainer(DBConfig.CONTAINERTODO);

                var partition = new PartitionKey(todo.TodoType);
                var information = await cosmosContainer
                                    .DeleteItemAsync<Model.Todo>(todo.Id,partition);
                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }
    }
}
