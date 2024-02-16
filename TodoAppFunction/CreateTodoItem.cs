using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TodoAppFunction
{
    public class CreateTodoItem
    {
        private readonly ILogger _logger;
        private readonly TodoDB _todoDB = TodoDB.Instance;

        public CreateTodoItem(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreateTodoItem>();
        }

        [Function("CreateTodoItem")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a POST request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"request body: {requestBody}");

            var newTodo = JsonSerializer.Deserialize<Todo>(requestBody);
            _logger.LogInformation($"Todo todo: {newTodo}");
            Todo insertedTodo = _todoDB.Add(newTodo);

            return new OkObjectResult(insertedTodo);
        }
    }
}
