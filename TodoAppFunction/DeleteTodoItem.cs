using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TodoAppFunction
{
    public class DeleteTodoItem
    {
        private readonly ILogger _logger;
        private readonly TodoDB _todoDB = TodoDB.Instance;

        public DeleteTodoItem(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DeleteTodoItem>();
        }

        [Function("DeleteTodoItem")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a DELETE request.");

            int todoId = int.Parse(req.Query["id"]);
            _logger.LogInformation($"todoId: {todoId}");

            var deletedTodo = _todoDB.Delete(todoId);

            if (deletedTodo == null) return new NotFoundObjectResult(404);
            return new OkObjectResult(deletedTodo);
        }
    }
}
