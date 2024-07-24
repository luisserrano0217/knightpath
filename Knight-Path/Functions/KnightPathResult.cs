using Azure.Data.Tables;
using Knight_Path.Dtos;
using Knight_Path.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Knight_Path.Functions
{
    public class KnightPathResult(ILogger<KnightPathResult> logger)
    {
        private readonly ILogger<KnightPathResult> _logger = logger;

        [Function("KnightPathResult")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "knightpath")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? operationId = req.Query["operationId"];

            if (string.IsNullOrEmpty(operationId))
            {
                var resp = req.CreateResponse(HttpStatusCode.BadRequest);
                resp.WriteString("Please pass an operationId on the query string");
                return resp;
            }


            var tableClient = new TableClient(new Uri(Environment.GetEnvironmentVariable("TableStorageUri")),
                "KnightPaths", new TableSharedKeyCredential(Environment.GetEnvironmentVariable("AccountName"),
                Environment.GetEnvironmentVariable("AccountKey")));

            var entity = await tableClient.GetEntityAsync<KnightPathEntity>("knightpath", operationId);
            if (entity == null)
            {
                //return new NotFoundObjectResult("Operation ID not found");
                var resp = req.CreateResponse(HttpStatusCode.NotFound);
                resp.WriteString("Operation ID not found");
                return resp;
            }

            var responseObject = new KnightPathResultDto
            {
                Starting = entity.Value.Source,
                Ending = entity.Value.Target,
                ShortestPath = entity.Value.ShortestPath,
                NumberOfMoves = entity.Value.NumberOfMoves,
                OperationId = entity.Value.RowKey
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(responseObject);

            return response;
        }
    }
}
