using System;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Knight_Path.Contracts;
using Knight_Path.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Knight_Path.Functions
{
    public class ProcessKnightPathRequest(ILogger<ProcessKnightPathRequest> logger, IKnightPathCalculator service)
    {
        private readonly ILogger<ProcessKnightPathRequest> _logger = logger;
        private readonly IKnightPathCalculator _service = service;

        [Function(nameof(ProcessKnightPathRequest))]
        public async Task RunAsync([QueueTrigger("knightpathqueue", Connection = "AzureWebJobsStorage")] QueueMessage operationId)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {operationId.MessageText}");

            try
            {
                var tableClient = new TableClient(new Uri(Environment.GetEnvironmentVariable("TableStorageUri")),
                    "KnightPaths", new TableSharedKeyCredential(Environment.GetEnvironmentVariable("AccountName"),
                    Environment.GetEnvironmentVariable("AccountKey")));

                var entity = await tableClient.GetEntityAsync<KnightPathEntity>("knightpath", operationId.MessageText);
                if (entity != null)
                {
                    var (path, moves) = _service.CalculateKnightPath(entity.Value.Source, entity.Value.Target);
                    entity.Value.ShortestPath = path;
                    entity.Value.NumberOfMoves = moves;

                    await tableClient.UpdateEntityAsync(entity.Value, entity.Value.ETag, TableUpdateMode.Replace);
                }
                else
                {
                    _logger.LogWarning($"Entity with operationId {operationId.MessageText} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process message with operationId: {operationId.MessageText}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
