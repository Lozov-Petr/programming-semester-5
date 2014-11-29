using System;
using System.Collections.Generic;
using QuickGraph;
using Graphviz4Net.Dot.AntlrParser;
using Graphviz4Net.Dot;
using System.IO;


namespace HamiltinianCompletion
{
    class Graph<V> : IGraph<V, UndirectedEdge<V>>
    {
        #region Variables

        public UndirectedGraph<int, UndirectedEdge<int>> graph;
        Dictionary<V, int> inInternal;
        V[] inExternal;

        static Random rnd = new Random();

        public bool IsDirected { get { return false; } }
        public bool AllowParallelEdges { get { return false; } }

        #endregion

        #region Constructors

        public Graph(V[] vertices)
        {
            var edgesCount = vertices.Length / 2;
            var edges = new UndirectedEdge<V>[edgesCount];
            for (int i = 0; i < edgesCount; ++i)
                edges[i] = new UndirectedEdge<V>(vertices[2 * i], vertices[2 * i + 1]);

            Create(edges, new V[0]);
        }

        public Graph(ICollection<UndirectedEdge<V>> edges)
        {
            Create(edges, new V[0]);
        }

        public Graph(ICollection<UndirectedEdge<V>> edges, ICollection<V> vertices)
        {
            Create(edges, vertices);
        }

        void Create(ICollection<UndirectedEdge<V>> edges, ICollection<V> vertices)
        {
            graph = new UndirectedGraph<int, UndirectedEdge<int>>();

            inInternal = new Dictionary<V, int>();

            int i = 0;

            foreach (var vertex in vertices)
                if (!inInternal.ContainsKey(vertex)) inInternal.Add(vertex, i++);

            foreach (var edge in edges)
            {
                if (!inInternal.ContainsKey(edge.Source)) inInternal.Add(edge.Source, i++);
                if (!inInternal.ContainsKey(edge.Target)) inInternal.Add(edge.Target, i++);
            }

            inExternal = new V[inInternal.Count];

            foreach (var pair in inInternal)
            {
                inExternal[pair.Value] = pair.Key;
                graph.AddVertex(pair.Value);
            }


            foreach (var edge in edges)
            {
                var newSource = inInternal[edge.Source];
                var newTarget = inInternal[edge.Target];

                if (newSource > newTarget) graph.AddEdge(new UndirectedEdge<int>(newTarget, newSource));
                else if (newSource < newTarget) graph.AddEdge(new UndirectedEdge<int>(newSource, newTarget));
            }
        }

        #endregion

        #region Contains

        internal bool ContainsEdge(int source, int target)
        {
            if (source < target) return graph.ContainsEdge(source, target);
            else if (source > target) return graph.ContainsEdge(target, source);
            else return false;
        }

        internal bool ContainsVertex(int vertex)
        {
            return vertex >= 0 && vertex < graph.VertexCount;
        }

        #endregion

        #region Read/Write

        public static Graph<string> ReadInDotFile(string filePath)
        {
            var parser = AntlrParserAdapter<string>.GetParser();
            var stream = new StreamReader(filePath);
            var dotGraph = parser.Parse(stream);

            var vertices = new List<string>();
            foreach (var vertex in dotGraph.Vertices) vertices.Add(vertex.Id);

            var edges = new List<UndirectedEdge<string>>();
            foreach (var objEdge in dotGraph.Edges)
            {
                var edge = (DotEdge<string>)objEdge;
                edges.Add(new UndirectedEdge<string>(edge.Source.Id, edge.Destination.Id));

            }
            stream.Close();
            return new Graph<string>(edges, vertices);
        }

        public void WriteToDotFile(string filePath)
        {
            var stream = new StreamWriter(filePath, false);
            stream.Write("graph g\r\n{\r\n");

            foreach (var vertex in inExternal)
                stream.Write(string.Concat(vertex, ";\r\n"));

            stream.Write("\r\n");

            foreach (var edge in graph.Edges)
                stream.Write(string.Concat(External(edge.Source), "--", External(edge.Target), ";\r\n"));

            stream.Write("}");

            stream.Close();
        }

        #endregion

        #region Internal/External

        int Internal(V vertex)
        {
            return inInternal[vertex];
        }

        List<int> Internal(List<V> vertices)
        {
            return vertices.ConvertAll(Internal);
        }

        V External(int vertex)
        {
            return inExternal[vertex];
        }

        List<V> External(List<int> vertices)
        {
            return vertices.ConvertAll(External);
        }

        #endregion

        #region Random Generation

        static public Graph<int> CreateRandomGraph(int vertexCount, float completeness)
        {
            var vertices = new int[vertexCount];
            for (int i = 0; i < vertexCount; ++i) vertices[i] = i;

            var allEdgeCount = vertexCount * (vertexCount - 1) / 2;
            var edgeCount = (int)((float)allEdgeCount * completeness);

            var edgesNumbers = new int[allEdgeCount];
            var edges = new UndirectedEdge<int>[edgeCount];

            for (int i = 0; i < allEdgeCount; ++i) edgesNumbers[i] = i;

            for (int i = allEdgeCount - 1; i > allEdgeCount - 1 - edgeCount; --i)
            {

                var randomNumber = rnd.Next(i + 1);

                var number = edgesNumbers[randomNumber];

                var target = (int)((Math.Sqrt(1 + 8 * number) + 1) / 2);
                var source = number - target * (target - 1) / 2;

                edges[allEdgeCount - i - 1] = new UndirectedEdge<int>(source, target);

                edgesNumbers[randomNumber] = edgesNumbers[i];
            }

            return new Graph<int>(edges, vertices);
        }

        #endregion

        #region Greedy Algorithm

        Completion<V> GreedyAlgorithm(Func<int, List<int>, Tuple<int, bool>> separator, List<V> startPath)
        {
            var path = Internal(startPath);

            if (path.Count == 0) path.Add(0);

            var tempRest = new bool[graph.VertexCount];
            for (int i = 0; i < tempRest.Length; ++i) tempRest[i] = true;

            int? prevVertex = null;
            int addOn = 0;

            foreach (var vertex in path)
            {
                if (prevVertex != null && !ContainsEdge((int)prevVertex, vertex)) ++addOn;

                prevVertex = vertex;
                if (tempRest[vertex]) tempRest[vertex] = false;
                else throw new Exception("Incorrect path");
            }

            var rest = new List<int>(graph.VertexCount - path.Count);
            for (int i = 0; i < tempRest.Length; ++i)
                if (tempRest[i]) rest.Add(i);

            int currVertex = path[path.Count - 1];

            while (path.Count != graph.VertexCount)
            {
                var tuple = separator(currVertex, rest);
                currVertex = tuple.Item1;
                path.Add(currVertex);
                rest.Remove(currVertex);
                if (!tuple.Item2) ++addOn;
            }

            if (!ContainsEdge(path[0], currVertex)) ++addOn;

            return new Completion<V>(External(path), addOn);
        }

        public Completion<V> SquareGreedyAlgorithm(List<V> startPath)
        {
            Func<int, List<int>, Tuple<int, bool>> separator = (currVertex, rest) =>
            {
                if (rest.Count == 0) throw new Exception("path is complete");

                foreach (var vertex in rest)
                    if (ContainsEdge(currVertex, vertex))
                        return new Tuple<int, bool>(vertex, true);

                var newVertex = rest[0];
                return new Tuple<int, bool>(newVertex, false);
            };

            return GreedyAlgorithm(separator, startPath);
        }

        public Completion<V> SquareGreedyAlgorithm()
        {
            return SquareGreedyAlgorithm(new List<V>());
        }

        public Completion<V> CubeGreedyAlgorithm(List<V> startPath)
        {
            Func<int, List<int>, Tuple<int, bool>> separator = (currVertex, rest) =>
            {
                if (rest.Count == 0) throw new Exception("path is complete");

                int? vertex10 = null;
                int? vertex01 = null;
                int? vertex00 = null;

                foreach (int vertex in rest)
                {
                    var Edge1IsExist = ContainsEdge(currVertex, vertex);
                    var Edge2IsExist = rest.Exists(vertex1 => ContainsEdge(vertex, vertex1));

                    if (Edge1IsExist)
                        if (Edge2IsExist) return new Tuple<int, bool>(vertex, true);
                        else vertex10 = vertex;
                    else if (Edge2IsExist) vertex01 = vertex;
                    else vertex00 = vertex;
                }


                if (vertex10 != null) return new Tuple<int, bool>((int)vertex10, true);
                if (vertex01 != null) return new Tuple<int, bool>((int)vertex01, false);
                return new Tuple<int, bool>((int)vertex00, false);
            };

            return GreedyAlgorithm(separator, startPath);
        }

        public Completion<V> CubeGreedyAlgorithm()
        {
            return CubeGreedyAlgorithm(new List<V>());
        }

        #endregion

        #region Brute-Force Search Algorithm

        public Completion<V> BruteForceSearchAlgorithm()
        {
            var notEnd = true;

            var bestCicle = new int[graph.VertexCount];
            bestCicle[0] = 0;
            var bestAddOn = graph.VertexCount;

            var perm = new Permutation(graph.VertexCount - 1);

            do
            {
                var currAddOn = 0;

                if (!ContainsEdge(0, perm.perm[0])) ++currAddOn;
                if (!ContainsEdge(0, perm.perm[graph.VertexCount - 2])) ++currAddOn;

                for (int i = 0; i < graph.VertexCount - 2; ++i)
                    if (!ContainsEdge(perm.perm[i], perm.perm[i + 1])) ++currAddOn;


                if (currAddOn < bestAddOn)
                {
                    bestAddOn = currAddOn;
                    for (int i = 1; i < graph.VertexCount; ++i)
                        bestCicle[i] = perm.perm[i - 1];
                    if (bestAddOn == 0) break;
                }

                notEnd = perm.Next();
            }
            while (notEnd);

            var resultCicle = new List<V>(graph.VertexCount);
            for (int i = 0; i < graph.VertexCount; ++i) resultCicle.Add(External(bestCicle[i]));

            return new Completion<V>(resultCicle, bestAddOn);
        }

        #endregion

        #region Branch And Bound Algorithm

        public Completion<V> BranchAndBoundAlgorithm()
        {
            if (graph.VertexCount <= 5) return BruteForceSearchAlgorithm();

            var firstCicle = SquareGreedyAlgorithm();

            var maxAddOn = firstCicle.AddOn;

            StructForBaBA Struct = new StructForBaBA(maxAddOn, graph.VertexCount, graph);

            var minAddOn = maxAddOn;
            Path bestCicle = null;

            var currPathAndAddOn = Struct.PopBest();

            while (currPathAndAddOn != null)
            {
                var currPath = currPathAndAddOn.Item1;
                var currAddOn = currPathAndAddOn.Item2;
                var length = currPathAndAddOn.Item3;
                var currVertex = currPath.Value;

                if (graph.VertexCount == length)
                {

                    var addOn = currAddOn + (ContainsEdge(currPath.FirstAncestor.Value, currVertex) ? 0 : 1);
                    if (addOn < minAddOn)
                    {
                        minAddOn = addOn;
                        bestCicle = currPath;
                        if (minAddOn == Struct.MinAddOn) break;
                        Struct.MaxAddOn = addOn;
                    }
                }
                else
                {
                    bool[] rest = new bool[graph.VertexCount];
                    for (int i = 0; i < graph.VertexCount; ++i) rest[i] = true;

                    for (var path = currPath; path != null; path = path.Parent) rest[path.Value] = false;


                    for (int i = 0; i < graph.VertexCount; ++i)
                        if (rest[i])
                        {
                            var newPath = new Path(i, currPath);
                            var newAddOn = currAddOn + (ContainsEdge(currVertex, i) ? 0 : 1);
                            Struct.Add(newPath, length + 1, newAddOn);
                        }
                }
                currPathAndAddOn = Struct.PopBest();
            }

            if (bestCicle == null) return firstCicle;

            var resultCicle = new List<V>(graph.VertexCount);

            for (var path = bestCicle; path != null; path = path.Parent) resultCicle.Add(External(path.Value));

            return new Completion<V>(resultCicle, minAddOn);
        }

        #endregion

        #region Genetic Algorithm

        public Completion<V> GeneticAlgorithm()
        {
            return GeneticAlgorithm(20, 5000, 200, 0.02d);
        }

        public Completion<V> GeneticAlgorithm(int selectionCount, int maxCountSteps, int maxUnchangetWaiting, double mutationProbability)
        {
            if (graph.VertexCount < 5) return BruteForceSearchAlgorithm();

            var count = selectionCount * (selectionCount + 1) / 2;

            var cicles = new StructForGA(graph.VertexCount, selectionCount, mutationProbability, ContainsEdge);

            cicles.CreateStartData();
            var bestCicles = cicles.Selection();
            var bestAddOn = cicles.BestAddOn();

            var countStepsAfterLastChange = 0;
            var countSteps = 0;

            var stepsAfterLastChangeControl = maxUnchangetWaiting > 0
                                            ? new Predicate<int>(c => c < maxUnchangetWaiting)
                                            : new Predicate<int>(c => true);

            var allStepsControl = maxCountSteps > 0
                                ? new Predicate<int>(c => c < maxCountSteps)
                                : new Predicate<int>(c => true);

            while (stepsAfterLastChangeControl(countStepsAfterLastChange) && allStepsControl(countSteps))
            {
                for (int i = 0; i < selectionCount - 1; ++i)
                    for (int j = i + 1; j < selectionCount; ++j)
                        cicles.Add(cicles.Crossover(bestCicles[i], bestCicles[j]));

                bestCicles = cicles.Selection();

                var newBestAddOn = cicles.BestAddOn();

                if (newBestAddOn < bestAddOn)
                {
                    bestAddOn = newBestAddOn;
                    countStepsAfterLastChange = 0;
                }
                else ++countStepsAfterLastChange;

                ++countSteps;

                if (bestAddOn == 0) break;
            }

            var completion = External(new List<int>(bestCicles[0]));

            return new Completion<V>(completion, bestAddOn);
        }

        #endregion
    }
}
