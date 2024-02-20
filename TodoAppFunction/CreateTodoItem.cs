using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TodoAppFunction.Model;

namespace TodoAppFunction
{
    public static class CreateTodoItem
    {
        [FunctionName("CreateTodoItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Todo")] HttpRequest req,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODO,
                Connection = DBConfig.CONNECTION)]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a CreateTodoItem request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"request body: {requestBody}");

            var newTodo = JsonConvert.DeserializeObject<Todo>(requestBody);
            log.LogInformation($"Todo todo: {newTodo}");

            await documentsOut.AddAsync(newTodo);
            await EventGridUtil.SendDataToEventGrid(newTodo, "Create/");
            return new OkObjectResult(newTodo);
        }
    }
}
