using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuickGraph;
using HamiltinianCompletion;


namespace Program
{
    class Program
    {

        static string help = "help:\r\nHamiltoniamCompletion.exe (BruteForceSearch | BranchAndBound | GreedyAlgorithm | Genetic) [INPUT path] [OUTPUT path]";
        
        static void Main(string[] args) 
        {
            if (args.Length == 3)
            {
                int algNumber;

                switch (args[0])
                {
                    case "BruteForceSearch":
                        algNumber = 0;
                        break;
                    case "BranchAndBound":
                        algNumber = 1;
                        break;
                    case "GreedyAlgorithm":
                        algNumber = 2;
                        break;
                    case "Genetic":
                        algNumber = 3;
                        break;
                    default:
                        Console.WriteLine(help);
                        return;
                }

                try
                {
                    var g = Graph<string>.ReadInDotFile(args[1]);

                    var res = algNumber == 0 ? g.BruteForceSearchAlgorithm()
                            : algNumber == 1 ? g.BranchAndBoundAlgorithm()
                            : algNumber == 2 ? g.SquareGreedyAlgorithm()
                            : g.GeneticAlgorithm();

                    g.AddCicle(res.Cicle);
                    g.WriteToDotFile(args[2]);

                    var stream = new StreamWriter(string.Concat(args[2], ".log"), false);
                    stream.Write(res.ToString());
                    stream.Close();
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File with graph not found");
                    return;
                }
                catch
                {
                    Console.WriteLine("Incorrect graph in file");
                    return;
                }
            }
            else if (args.Length == 1 && args[0] == "StartTests") AllTest();

            else if (args.Length == 8 && args[0] == "StartTests") AllTest(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]),
                                                                          int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[7]),
                                                                          int.Parse(args[6]));
            else Console.WriteLine(help);
        }

        static void AllTest(int minNumberOfRunningAlgorithm = 0,
                            int maxNumberOfRunningAlgorithm = 4,
                            int countTests = 50,
                            int minCountVertices = 6,
                            int maxCountVertices = 10,
                            int minCompleteness = 1,
                            int maxCompleteness = 9)
        {

            #region Initialize

            var countSteps1 = maxCountVertices - minCountVertices + 1;
            var countSteps2 = maxCompleteness - minCompleteness + 1;

            var allTimes = new long[countSteps1][][][];
            var allAddOn = new int[countSteps1][][][];

            for (int i = 0; i < countSteps1; ++i)
            {
                allTimes[i] = new long[countSteps2][][];
                allAddOn[i] = new int[countSteps2][][];
                for (int j = 0; j < countSteps2; ++j)
                {
                    allTimes[i][j] = new long[countTests][];
                    allAddOn[i][j] = new int[countTests][];

                    for (int k = 0; k < countTests; ++k)
                    {
                        allTimes[i][j][k] = new long[maxNumberOfRunningAlgorithm];
                        allAddOn[i][j][k] = new int[maxNumberOfRunningAlgorithm];
                    }
                }
            }

            var sw = new Stopwatch();
            Completion<int> res;

            #endregion

            #region Computation

            for (int i = 0; i < countSteps1; ++i)
                for (int j = 0; j < countSteps2; ++j)
                    for (int k = 0; k < countTests; ++k)
                    {
                        var g = Graph<int>.CreateRandomGraph(minCountVertices + i, (float)(minCompleteness + j) / 10f);
                        var algorithmes = new Func<Completion<int>>[]  { g.BruteForceSearchAlgorithm, g.BranchAndBoundAlgorithm,
                                                                         g.SquareGreedyAlgorithm,     g.GeneticAlgorithm };

                        Console.WriteLine("Graph {0} {1} {2} create", i + minCountVertices, j + minCompleteness, k + 1);
                        
                        for (int l = minNumberOfRunningAlgorithm; l < maxNumberOfRunningAlgorithm; ++l)
                        {

                            sw.Restart();
                            res = algorithmes[l]();
                            sw.Stop();

                            allTimes[i][j][k][l] = sw.ElapsedMilliseconds;
                            allAddOn[i][j][k][l] = res.AddOn;

                            Console.WriteLine("Step {0} {1} {2} {3} done", i + minCountVertices, j + minCompleteness, k + 1, l + 1);
                        }
                    }

            #endregion

            #region Calculate results

            var averageTimes = new long[maxNumberOfRunningAlgorithm][];
            var averageMiss = new float[maxNumberOfRunningAlgorithm][];

            for (int i = minNumberOfRunningAlgorithm; i < maxNumberOfRunningAlgorithm; ++i)
            {
                averageTimes[i] = new long[countSteps1];
                averageMiss[i] = new float[countSteps1];
                for (int j = 0; j < countSteps1; ++j)
                {
                    long accTimes = 0;
                    int accMiss = 0;

                    for (int k = 0; k < countSteps2; ++k)
                        for (int l = 0; l < countTests; ++l)
                        {
                            accTimes += allTimes[j][k][l][i];
                            accMiss += allAddOn[j][k][l][i] - allAddOn[j][k][l][1];
                        }

                    averageTimes[i][j] = accTimes / (countSteps2 * countTests);
                    averageMiss[i][j] = (float)accMiss / (float)(countSteps2 * countTests);

                }
            }

            #endregion

            #region Output results

            var stream = new StreamWriter("resultOfTests.txt", true);

            var algNames = new string[] { "BruteForceSearch", "BranchAndBound", "GreedyAlgorithm", "Genetic" };

            stream.WriteLine();
            stream.WriteLine();
            stream.WriteLine("Average time");
            stream.WriteLine("{");

            for (int i = minNumberOfRunningAlgorithm; i < maxNumberOfRunningAlgorithm; ++i)
            {
                stream.WriteLine(string.Format("    {0}", algNames[i]));
                stream.WriteLine("    {");
                for (int j = 0; j < countSteps1; ++j)
                    stream.WriteLine(string.Format("        {0}: {1}", j + minCountVertices, averageTimes[i][j]));
                stream.WriteLine("    }");
            }

            stream.WriteLine("}");
            stream.WriteLine();

            stream.WriteLine("Average miss");
            stream.WriteLine("{");

            for (int i = 2; i < maxNumberOfRunningAlgorithm; ++i)
            {
                stream.WriteLine(string.Format("    {0}", algNames[i]));
                stream.WriteLine("    {");
                for (int j = 0; j < countSteps1; ++j)
                    stream.WriteLine(string.Format("        {0}: {1}", j + minCountVertices, averageMiss[i][j]));
                stream.WriteLine("    }");
            }
            stream.WriteLine("}");

            stream.Close();

            #endregion
        }
    }
}
