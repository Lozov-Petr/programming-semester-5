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
        static float isIncest = 0.95f;

        public StructForGA(int _vertexCount, int _selectionCount, double _mutationProbability, Func<int, int, bool> _contains)
        {
            contains = _contains;
            selectionCount = _selectionCount;
            vertexCount = _vertexCount;
            mutationProbability = _mutationProbability;

            count = 3 * selectionCount;
            cicles = new int[vertexCount + 2][][];
            lengths = new int[vertexCount + 2];
            for (int i = 0; i <= vertexCount + 1; ++i) cicles[i] = new int[count][];
        }

        public int TargetFunction(int[] cicle)
        {
            var addOn = 0;

            for (int i = 1; i < vertexCount; ++i)
                if (!contains(cicle[i - 1], cicle[i])) ++addOn;

            if (!contains(0, cicle[0])) ++addOn;
            if (!contains(0, cicle[vertexCount - 1])) ++addOn;

            return addOn;
        }

        public int[] RandomCicle()
        {
            var cicle = new int[vertexCount];
            var rest = new int[vertexCount];

            for (int i = 0; i < vertexCount; ++i) rest[i] = i + 1;

            for (int i = vertexCount - 1; i >= 0; --i)
            {
                var rndIndex = rnd.Next(i + 1);
                cicle[i] = rest[rndIndex];
                rest[rndIndex] = rest[i];
            }

            return cicle;
        }

        public bool IsRelatives(int[] cicle1, int[] cicle2)
        {
            int minCorrectDiffCount = (int)((1f - isIncest) * vertexCount);
            int diffCount = 0;

            for (int i = 0; i < vertexCount; ++i)
            {
                diffCount += cicle1[i] == cicle2[i] ? 0 : 1;
                if (diffCount > minCorrectDiffCount) return false;
            }
            return true;
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

            for (int i = 0; i <= vertexCount + 1; ++i)
                if (index == selectionCount)
                {
                    cicles[i] = new int[count][];
                    lengths[i] = 0;
                }
                else
                {
                    while (lengths[i] > 0 && IsBad(cicles[i][lengths[i] - 1])) cicles[i][--lengths[i]] = null;

                    for (int j = lengths[i] - 1; j > 0 ; --j)
                    {
                        bestCicles[index++] = cicles[i][j];
                        
                        if (IsBad(cicles[i][j - 1]))
                        {
                            cicles[i][--j] = cicles[i][lengths[i] - 1];
                            cicles[i][--lengths[i]] = null;
                        }
                        if (index == selectionCount)
                        {
                            var truncatedArr = new int[count][];
                            for (int k = 0; k <= lengths[i] - j - 1; ++k) truncatedArr[k] = cicles[i][j + k];
                            cicles[i] = truncatedArr;
                            lengths[i] = lengths[i] - j;
                            break;
                        }
                    }

                    if (lengths[i] > 0 && index < selectionCount) bestCicles[index++] = cicles[i][0];
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

            if (IsRelatives(cicle1, cicle2))
            {
                MakeBad(cicle2);
                return RandomCicle();
            }
            
            var result = new int[vertexCount];
            var rest = new int[vertexCount];
            var restIndexes = new int[vertexCount];

            for (int i = 0; i < vertexCount; ++i)
            {
                rest[i] = i + 1;
                restIndexes[i] = i;
            }

            for (int i = 0; i < vertexCount; ++i)
            {
                if (restIndexes[cicle1[i] - 1] >= 0)
                    if (restIndexes[cicle2[i] - 1] >= 0) result[i] = rnd.Next(2) == 0 ? cicle1[i] : cicle2[i];
                    else result[i] = cicle1[i];
                else
                    if (restIndexes[cicle2[i] - 1] >= 0) result[i] = cicle2[i];
                    else result[i] = rest[rnd.Next(vertexCount - i - 1)];

                var index = restIndexes[result[i] - 1];
                if (index == vertexCount - i - 1) restIndexes[result[i] - 1] = -1;
                else
                {
                    rest[index] = rest[vertexCount - i - 1];
                    restIndexes[rest[index] - 1] = restIndexes[result[i] - 1];
                    restIndexes[result[i] - 1] = -1;
                }
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
