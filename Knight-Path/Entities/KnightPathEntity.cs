using Azure;
using Azure.Data.Tables;

namespace Knight_Path.Entities
{
    public class KnightPathEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Source { get; set; }
        public string Target { get; set; }
        public string ShortestPath { get; set; }
        public int NumberOfMoves { get; set; }

        public KnightPathEntity() { }

        public KnightPathEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
