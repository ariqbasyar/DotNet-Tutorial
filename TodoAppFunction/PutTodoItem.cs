using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TodoAppFunction
{
    public class PutTodoItem
    {
        private readonly ILogger _logger;
        private readonly TodoDB _todoDB = TodoDB.Instance;

        public PutTodoItem(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PutTodoItem>();
        }

        [Function("PutTodoItem")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a PUT request.");

            int todoId = int.Parse(req.Query["id"]);
            _logger.LogInformation($"todoId: {todoId}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"request body: {requestBody}");

            var putTodo = JsonSerializer.Deserialize<Todo>(requestBody);
            _logger.LogInformation($"Todo todo: {putTodo}");

            var updatedTodo = _todoDB.Put(todoId, putTodo);
            string strUpdatedTodo = JsonSerializer.Serialize(updatedTodo);
            _logger.LogInformation($"Str Todo todo: {strUpdatedTodo}");

            if (updatedTodo == null) return new NotFoundObjectResult(404);
            return new OkObjectResult(updatedTodo);
        }
    }
}
