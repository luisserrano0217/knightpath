using Knight_Path.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight_Path.Services
{
    internal class KnightPathCalculator : IKnightPathCalculator
    {
        private static readonly int[] dx = [2, 2, 1, 1, -1, -1, -2, -2];
        private static readonly int[] dy = [1, -1, 2, -2, 2, -2, 1, -1];

        private static readonly Dictionary<char, int> letterToIndex = new()
        {
            {'A', 0}, {'B', 1}, {'C', 2}, {'D', 3}, {'E', 4}, {'F', 5}, {'G', 6}, {'H', 7}
        };

        private static readonly Dictionary<int, char> indexToLetter = new()
        {
            {0, 'A'}, {1, 'B'}, {2, 'C'}, {3, 'D'}, {4, 'E'}, {5, 'F'}, {6, 'G'}, {7, 'H'}
        };

        public (string path, int moves) CalculateKnightPath(string source, string target)
        {
            var start = ConvertChessToCoords(source);
            var end = ConvertChessToCoords(target);

            if (start == end)
            {
                return (source, 0);
            }

            var queue = new Queue<(int x, int y, int dist, List<string> path)>();
            queue.Enqueue((start.x, start.y, 0, new List<string> { source }));

            var visited = new bool[8, 8];
            visited[start.x, start.y] = true;

            while (queue.Count > 0)
            {
                var (x, y, dist, path) = queue.Dequeue();

                for (int i = 0; i < 8; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (nx >= 0 && ny >= 0 && nx < 8 && ny < 8 && !visited[nx, ny])
                    {
                        var newPath = new List<string>(path) { ConvertCoordsToChess(nx, ny) };

                        if (nx == end.x && ny == end.y)
                        {
                            return (string.Join(":", newPath), dist + 1);
                        }

                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny, dist + 1, newPath));
                    }
                }
            }

            throw new InvalidOperationException("No valid path found");
        }

        public bool IsValidChessPosition(string position)
        {
            if (position.Length != 2)
            {
                return false;
            }

            char file = position[0];
            char rank = position[1];

            return file >= 'A' && file <= 'H' && rank >= '1' && rank <= '8';
        }

        private static (int x, int y) ConvertChessToCoords(string position)
        {
            int x = letterToIndex[position[0]];
            int y = position[1] - '1';
            return (x, y);
        }

        private static string ConvertCoordsToChess(int x, int y)
        {
            return $"{indexToLetter[x]}{y + 1}";
        }

    }
}
