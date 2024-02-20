using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace TodoAppFunction
{
    public static class GetTodoItems
    {
        [FunctionName("GetTodoItem")]
        public static IActionResult GetTodoItem(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Todo/{todoType}/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODO,
                Connection = DBConfig.CONNECTION,
                Id = "{id}",
                PartitionKey = "{todoType}")]Model.Todo todo,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a GetTodoItem request.");

            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodoByType")]
        public static IActionResult GetTodoByType(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Todo/{todoType}")] HttpRequest req,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODO,
                Connection = DBConfig.CONNECTION,
                PartitionKey = "{todoType}")] IEnumerable<Model.Todo> todoItems,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a GetTodoByType request.");

            return new OkObjectResult(todoItems.ToList());
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Todos")] HttpRequest req,
            [CosmosDB(
                databaseName: DBConfig.DATABASE,
                containerName: DBConfig.CONTAINERTODO,
                Connection = DBConfig.CONNECTION)] IEnumerable<Model.Todo> todoItems,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a GetTodoByType request.");

            return new OkObjectResult(todoItems.ToList());
        }
    }
}
