using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight_Path.Contracts
{
    public interface IKnightPathCalculator
    {
        public (string path, int moves) CalculateKnightPath(string source, string target);
        public bool IsValidChessPosition(string position);
    }
}
