using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CosmosDBAndAzureFunctionPDP
{
    public class AzureFunctionCRUDwithCOSMOSDBController
    {
        private readonly ILogger<AzureFunctionCRUDwithCOSMOSDBController> _logger;
        const string _DATABASE = "Profile";
        const string _CONTAINER = "User";
        private string CONNECTIONSTRING = Environment.GetEnvironmentVariable("CosmosDBConnection");

        public AzureFunctionCRUDwithCOSMOSDBController(ILogger<AzureFunctionCRUDwithCOSMOSDBController> logger)
        {
            _logger = logger;
        }

        [Function("ReturnModelJSON")]
        public IActionResult ResturnModelJSON([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a ModelJSON request.");

            var result = new Model.User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "sample@company.com",
                Name = "Si Kancil",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            return new OkObjectResult(result);
        }

        /// <summary>
        /// Create User data given name and email.
        /// User is saved to db manually via SDK COSMOS v4.5
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("CreateUserData")]
        public async Task<IActionResult> CreateUserData([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a POST request.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var postUserData = JsonConvert.DeserializeObject<Model.User>(requestBody);

                postUserData.Id = Guid.NewGuid().ToString();
                postUserData.CreatedDate = DateTime.Now;
                postUserData.ModifiedDate = DateTime.Now;

                var client = new CosmosClient(CONNECTIONSTRING);
                Container cosmosContainer = client.GetDatabase(_DATABASE).GetContainer(_CONTAINER);

                var createdItem = await cosmosContainer.CreateItemAsync(postUserData);
                return new OkObjectResult(createdItem.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Prepare data using Binding
        /// Get data by Id using Binding directly
        /// </summary>
        /// <param name="req"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Function("GetSpecificUserDataById")]
        public IActionResult GetSpecificUserDataById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/Id/{id}")] HttpRequest req,
            [CosmosDBInput(
                databaseName: _DATABASE,
                containerName: _CONTAINER,
                Connection  = "CosmosDBConnection",
                Id = "{id}",
                PartitionKey = "{id}")] Model.User user)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a GetById request.");
                _logger.LogInformation(user.Name);
                return new OkObjectResult(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Get data using Document Client sdk v2 (and v4 soon)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Function("GetListUserData")]
        public async Task<IActionResult> GetListUserData(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/List")] HttpRequest req,
            [CosmosDBInput(Connection = "CosmosDBConnection")] DocumentClient client)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a GetList request.");

                var result = new List<Model.User>();

                Uri collectUri = UriFactory.CreateDocumentCollectionUri(_DATABASE, _CONTAINER);
                var options = new FeedOptions() { EnableCrossPartitionQuery = true };

                IDocumentQuery<Model.User> query = client.CreateDocumentQuery<Model.User>(collectUri, options)
                    .Where(p => p != null)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    foreach (Model.User item in await query.ExecuteNextAsync())
                    {
                        _logger.LogInformation(item.Name);
                        result.Add(item);
                    }
                }

                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Prepare Cosmos using SDK
        /// Get data using linq
        /// Update data using client V2
        /// </summary>
        /// <param name="req"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Function("UpdateSpecificUserDataById")]
        public async Task<IActionResult> UpdateSpecificUserDataById(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "User/Id/{id}")] HttpRequest req,
            [CosmosDBInput(Connection = "CosmosDBConnection")] DocumentClient client,
            string id)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a GetList request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var postData = JsonConvert.DeserializeObject<Model.User>(requestBody);
                postData.Id = id;

                Uri collectUri = UriFactory.CreateDocumentUri(_DATABASE, _CONTAINER, id);

                var result = await client.ReplaceDocumentAsync(collectUri, postData);

                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Prepare data using Binding
        /// Get data by Id using Binding directly
        /// Delete the User
        /// </summary>
        /// <param name="req"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Function("DeleteSpecificUserDataById")]
        public async Task<IActionResult> DeleteSpecificUserDataById(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "User/Id/{id}")] HttpRequest req,
            [CosmosDBInput(
                databaseName: _DATABASE,
                containerName: _CONTAINER,
                Connection  = "CosmosDBConnection",
                Id = "{id}",
                PartitionKey = "{id}")] Model.User user)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a DeleteById request.");
                if (user == null) throw new ArgumentNullException("User not found.");
                var client = new CosmosClient(CONNECTIONSTRING);
                Container cosmosContainer = client.GetDatabase(_DATABASE).GetContainer(_CONTAINER);

                var information = await cosmosContainer.DeleteItemAsync<Model.User>(user.Id, new PartitionKey(user.Id));
                return new OkObjectResult(information);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
                throw;
            }
        }
    }
}
