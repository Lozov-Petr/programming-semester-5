using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuickGraph;
using HamiltinianCompletion;


namespace Program
{
    class Program
    {

        static void Main4(string[] args) // Просто main 
        {
            var g = Graph<int>.CreateRandomGraph(10000, 0.8911f);
            g.WriteToDotFile("a.dot");
        }

        static void Main3(string[] args) // Скорости точных 
        {

            var g = Graph<int>.CreateRandomGraph(16, 0.3f);

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            var res1 = g.BranchAndBoundAlgorithm();
            sw.Stop();

            var t1 = sw.ElapsedMilliseconds;

            sw.Restart();
            var res2 = g.BruteForceSearchAlgorithm();
            sw.Stop();

            var t2 = sw.ElapsedMilliseconds;        
        }

        static void Main(string[] args) // Сравнение всех на неточность 
        {
            //int realAcc = 0;
            int acc1 = 0;
            //int miss1 = 0;
            int acc2 = 0;
            //int miss2 = 0;
            int acc3 = 0;
            //int miss3 = 0;

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 50; ++i)
            {
                Console.WriteLine("Start {0}", i + 1);
                //sw.Restart();
                var g = Graph<int>.CreateRandomGraph(1000, 0.8f);
                //sw.Stop();
                //Console.WriteLine("    CreateRandomGraph           ({0})", sw.ElapsedMilliseconds);
                //sw.Restart();
                //var t0 = g.BranchAndBoundAlgorithm();
                //sw.Stop();
                //Console.WriteLine("    BranchAndBoundAlgorithm ({0}) ({1})", t0.AddOn, sw.ElapsedMilliseconds);
                sw.Restart();
                var t2 = g.CubeGreedyAlgorithm();
                sw.Stop();
                Console.WriteLine("    CubeGreedyAlgorithm     ({0}) ({1})", t2.AddOn, sw.ElapsedMilliseconds);
                sw.Restart();
                var t1 = g.SquareGreedyAlgorithm();
                sw.Stop();
                Console.WriteLine("    SquareGreedyAlgorithm   ({0}) ({1})", t1.AddOn, sw.ElapsedMilliseconds);
                sw.Restart();
                var t3 = g.GeneticAlgorithm();
                sw.Stop();
                Console.WriteLine("    GeneticAlgorithm        ({0}) ({1})", t3.AddOn, sw.ElapsedMilliseconds);
                //realAcc += t0.AddOn;
                acc1 += t1.AddOn;
                acc2 += t2.AddOn;
                acc3 += t3.AddOn;
                //miss1 += t1.AddOn != t0.AddOn ? 1 : 0;
                //miss2 += t2.AddOn != t0.AddOn ? 1 : 0;
                //miss3 += t3.AddOn != t0.AddOn ? 1 : 0;
                Console.WriteLine("End\n");
            }
            Console.Read();
        }

        static void Main1(string[] args) // Перестановки 
        {
            var p = new Permutation(7);

            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}\n",
                p.perm[0], p.perm[1], p.perm[2], p.perm[3], p.perm[4], p.perm[5], p.perm[6]);

            while (p.Next()) Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}\n",
                                p.perm[0], p.perm[1], p.perm[2], p.perm[3], p.perm[4], p.perm[5], p.perm[6]);
        }
    }
}
