using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight_Path.Dtos
{
    public class KnightPathResultDto
    {
        public string? Starting { get; set; }
        public string? Ending { get; set; }
        public string? ShortestPath { get; set; }
        public int NumberOfMoves { get; set; }
        public string? OperationId { get; set; }

    }
}
