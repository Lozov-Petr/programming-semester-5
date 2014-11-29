using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamiltinianCompletion
{
    class StructForGA
    {
        int vertexCount;
        int selectionCount;
        double mutationProbability;
        Func<int, int, bool> contains;

        int count;
        public int[][][] cicles;
        public int[] lengths;

        static Random rnd = new Random();

        public StructForGA(int _vertexCount, int _selectionCount, double _mutationProbability, Func<int, int, bool> _contains)
        {
            contains = _contains;
            selectionCount = _selectionCount;
            vertexCount = _vertexCount;
            mutationProbability = _mutationProbability;

            count = selectionCount * (selectionCount + 1) / 2;
            cicles = new int[vertexCount + 1][][];
            lengths = new int[vertexCount + 1];
            for (int i = 0; i <= vertexCount; ++i) cicles[i] = new int[count][];
        }

        public int TargetFunction(int[] cicle)
        {
            var addOn = 0;

            for (int i = 1; i < vertexCount; ++i)
                if (!contains(cicle[i - 1], cicle[i])) ++addOn;

            if (!contains(cicle[0], cicle[vertexCount - 1])) ++addOn;

            return addOn;
        }

        public int[] RandomCicle()
        {
            var cicle = new int[vertexCount];
            var rest = new int[vertexCount];

            for (int i = 0; i < vertexCount; ++i) rest[i] = i;

            for (int i = vertexCount - 1; i >= 0; --i)
            {
                var rndIndex = rnd.Next(i + 1);
                cicle[i] = rest[rndIndex];
                rest[rndIndex] = rest[i];
            }

            return cicle;
        }

        public bool EqualCicles(int[] cicle1, int[] cicle2)
        {
            bool result = true;

            for (int i = 0; result && i < vertexCount; ++i)
                result = cicle1[i] == cicle2[i];

            return result;
        }

        public void Add(int[] cicle)
        {
            int addOn = TargetFunction(cicle);
            cicles[addOn][lengths[addOn]++] = cicle;
        }        
        
        public void CreateStartData()
        {
            for (int i = 0; i < count; ++i) Add(RandomCicle());
        }

        public int BestAddOn()
        {
            var bestAddOn = 0;
            while (lengths[bestAddOn] == 0) ++bestAddOn;

            return bestAddOn;
        }

        public int[][] Selection()
        {
            var index = 0;
            var bestCicles = new int[selectionCount][];

            for (int i = 0; i <= vertexCount; ++i)
                if (index == selectionCount)
                {
                    cicles[i] = new int[count][];
                    lengths[i] = 0;
                }
                else
                {
                    while (lengths[i] > 0 && IsBad(cicles[i][lengths[i] - 1])) cicles[i][--lengths[i]] = null;

                    for (int j = 0; j < lengths[i]; ++j)
                    {
                        if (IsBad(cicles[i][j]))
                        {
                            cicles[i][j] = cicles[i][lengths[i] - 1];
                            cicles[i][--lengths[i]] = null;
                            while (lengths[i] > 0 && IsBad(cicles[i][lengths[i] - 1])) cicles[i][--lengths[i]] = null;
                        }
                        bestCicles[index++] = cicles[i][j];
                        if (index == selectionCount)
                        {
                            var truncatedArr = new int[count][];
                            for (int k = 0; k <= j; ++k) truncatedArr[k] = cicles[i][k];
                            cicles[i] = truncatedArr;
                            lengths[i] = j + 1;
                            break;
                        }
                    }
                }
            return bestCicles;
        }

        public void Mutation(int[] c)
        {
            var i1 = rnd.Next(vertexCount - 1);
            var i2 = rnd.Next(vertexCount - 2);
            if (i2 >= i1) ++i2;

            var temp = c[i1];
            c[i1] = c[i2];
            c[i2] = temp;
        }

        public int[] Crossover(int[] cicle1, int[] cicle2)
        {
            
            if (IsBad(cicle1) || IsBad(cicle2)) return RandomCicle();

            if (EqualCicles(cicle1, cicle2))
            {
                MakeBad(cicle2);
                return RandomCicle();
            }
            
            var result = new int[vertexCount];
            var rest = new bool[vertexCount];

            for (int i = 0; i < vertexCount; ++i) rest[i] = true;

            for (int i = 0; i < vertexCount; ++i)
            {
                if (rest[cicle1[i]])
                    if (rest[cicle2[i]]) result[i] = rnd.Next(2) == 0 ? cicle1[i] : cicle2[i];
                    else result[i] = cicle1[i];
                else
                    if (rest[cicle2[i]]) result[i] = cicle2[i];
                    else
                    {
                        var goodVertex = 0;
                        while (!rest[goodVertex]) ++goodVertex;
                        result[i] = goodVertex;
                    }

                rest[result[i]] = false;
            }

            if (rnd.NextDouble() < mutationProbability) Mutation(result);

            return result;
        }

        void MakeBad(int[] cicle)
        {
            cicle[0] = -1;
        }

        bool IsBad(int[] cicle)
        {
            return cicle[0] == -1;
        }
    }
}
