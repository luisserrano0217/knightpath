using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Knight_Path.Entities;
using Knight_Path.Dtos;
using Knight_Path.Contracts;

namespace Knight_Path.Functions
{
    public class KnightPathRequest(ILogger<KnightPathResult> logger, IKnightPathCalculator service)
    {
        //private readonly ILogger _logger = loggerFactory.CreateLogger<KnightPathRequest>();
        private readonly ILogger<KnightPathResult> _logger = logger;
        private readonly IKnightPathCalculator _service = service;

        [Function("KnightPathRequest")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "knightpath")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? source = req.Query["source"];
            string? target = req.Query["target"];

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target) ||
                !_service.IsValidChessPosition(source) || !_service.IsValidChessPosition(target))
            {
                var resp = req.CreateResponse(HttpStatusCode.BadRequest);
                resp.WriteString("Please pass both source and target on the query string");
                return resp;
            }

            string operationId = Guid.NewGuid().ToString();

            var tableClient = new TableClient(new Uri(Environment.GetEnvironmentVariable("TableStorageUri")),
                "KnightPaths", new TableSharedKeyCredential(Environment.GetEnvironmentVariable("AccountName"),
                Environment.GetEnvironmentVariable("AccountKey")));

            var entity = new KnightPathEntity("knightpath", operationId)
            {
                Source = source,
                Target = target
            };

            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(entity);

            var queueClient = new QueueClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "knightpathqueue", new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(operationId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new OperationIdResponseDto { OperationId = operationId, Message = "Please query it to find your results." });

            return response;
        }
    }
}
