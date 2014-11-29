using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace HamiltinianCompletion
{
    class Path
    {
        public int Value;
        public Path Parent;
        public Path FirstAncestor;

        public Path(int value, Path parent, Path firstAncestor)
        {
            Value = value;
            Parent = parent;
            FirstAncestor = firstAncestor;
        }

        public Path(int value, Path parent)
        {
            Value = value;
            Parent = parent;
            FirstAncestor = parent.FirstAncestor;
        }

    }

    class StructForBaBA
    {
        Stack<Path>[][] Struct;
        int[] MaxLengthsOfPath;
        public int MaxAddOn;
        public int MinAddOn;
        int VertexCount;

        public StructForBaBA(int maxAddOn, int vertexCount, UndirectedGraph<int, UndirectedEdge<int>> graph)
        {
            MaxAddOn = maxAddOn;
            MinAddOn = maxAddOn;
            VertexCount = vertexCount;
           
            Struct = new Stack<Path>[maxAddOn + 1][];
            MaxLengthsOfPath = new int[maxAddOn + 1];

            for (int i = 0; i <= maxAddOn; ++i)
            {
                Struct[i] = new Stack<Path>[vertexCount + 1];
                for (int j = 0; j <= vertexCount; ++j)
                    Struct[i][j] = new Stack<Path>();
            }

            for (int vertexA = 1; vertexA < vertexCount - 1; ++vertexA)
            {
                var pathA = new Path(vertexA, null, null);

                for (int vertexB = vertexA + 1; vertexB < VertexCount; ++vertexB)
                {
                    int addOn = (graph.ContainsEdge(0, vertexA) ? 0 : 1) + (graph.ContainsEdge(0, vertexB) ? 0 : 1);
                    var path0 = new Path(0, pathA, pathA);
                    var pathB = new Path(vertexB, path0);
                    this.Add(pathB, 3, addOn);
                }
            }
        }

        public bool Add(Path path, int length, int addOn)
        {   
            if (addOn > MaxAddOn || length == 0 || length > VertexCount) return false;

            Struct[addOn][length].Push(path);

            if (addOn < MinAddOn) MinAddOn = addOn;
            if (length > MaxLengthsOfPath[addOn]) MaxLengthsOfPath[addOn] = length;

            return true;
        }

        public Tuple<Path, int, int> PopBest()
        {
            var maxLength = MaxLengthsOfPath[MinAddOn];
            var pathsWithMinAddOn = Struct[MinAddOn];
            var bestPaths = pathsWithMinAddOn[maxLength];

            while (bestPaths.Count == 0 && (MinAddOn != MaxAddOn || MaxLengthsOfPath[0] != 0))
            {
                if (maxLength > 0)
                {
                    --MaxLengthsOfPath[MinAddOn];
                    --maxLength;
                }
                else
                {
                    ++MinAddOn;
                    pathsWithMinAddOn = Struct[MinAddOn];
                    maxLength = MaxLengthsOfPath[MinAddOn];
                }
                bestPaths = pathsWithMinAddOn[maxLength];
            }

            if (bestPaths.Count == 0) return null;

            return new Tuple<Path, int, int>(bestPaths.Pop(), MinAddOn, maxLength);
        }
    }
}
