using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TodoAppFunction
{
    public class GetTodoItems
    {
        private readonly ILogger _logger;
        private readonly TodoDB _todoDB = TodoDB.Instance;

        public GetTodoItems(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetTodoItems>();
        }

        [Function("GetTodoItems")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a GET request.");

            return new OkObjectResult(_todoDB.Get());
        }
    }
}
