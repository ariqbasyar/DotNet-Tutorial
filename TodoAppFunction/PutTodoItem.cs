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
    public static class PutTodoItem
    {
        [FunctionName("PutTodoItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Todo/{todoType}/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINER,
                Connection = DBConfig.CONNECTION,
                Id = "{id}",
                PartitionKey = "{todoType}")]Model.Todo oldTodo,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed an UpdateTodoItem request.");

                if (oldTodo is null) return new NotFoundResult();
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var newTodo = JsonConvert.DeserializeObject<Model.Todo>(requestBody);
                log.LogWarning($"old todo {JsonConvert.SerializeObject(oldTodo)}");
                log.LogWarning($"new todo {JsonConvert.SerializeObject(newTodo)}");
                var changedPartitionKey = oldTodo.TodoType != newTodo.TodoType;
                var oldPartitionKey = changedPartitionKey ? oldTodo.TodoType : null;
                var updatedTodo = oldTodo.Update(newTodo);
                log.LogWarning($"updated todo {JsonConvert.SerializeObject(updatedTodo)}");

                var client = new CosmosClient(DBConfig.CONNECTIONSTRING);
                Container cosmosContainer = client.GetDatabase(DBConfig.DATABASE)
                                                  .GetContainer(DBConfig.CONTAINER);

                if (changedPartitionKey)
                {
                    var oldPartition = new PartitionKey(oldPartitionKey);
                    await cosmosContainer.DeleteItemAsync<Model.Todo>(oldTodo.Id, oldPartition);
                    var newPartition = new PartitionKey(updatedTodo.TodoType);
                    await cosmosContainer.CreateItemAsync(updatedTodo, newPartition);
                }
                else
                {
                    await cosmosContainer.ReplaceItemAsync(updatedTodo, oldTodo.Id);
                }
                return new OkObjectResult(updatedTodo);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
                throw;
            }
        }
    }
}
