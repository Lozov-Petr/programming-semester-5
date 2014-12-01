namespace HamiltinianCompletion
{
    class Permutation
    {
        public int[] perm;

        public Permutation(int size)
        {
            perm = new int[size];
            perm[0] = 1;
            perm[size - 1] = 2;

            for (int i = 1; i < size - 1; ++i)
                perm[i] = i + 2;
        }

        void Swap(int i, int j)
        {
            var temp = perm[i];
            perm[i] = perm[j];
            perm[j] = temp;
        }

        public bool Next()
        {  
            var size = perm.Length;

            var index1 = size - 3;
            if (index1 < 0) return false;

            while (index1 > 0 && perm[index1] > perm[index1 + 1]) --index1;

            if (index1 == 0)
            {   
                var isCorrect = false;

                if (perm[size - 1] != size)
                {
                    ++perm[size - 1];
                    isCorrect = true;
                }
                else if (perm[0] != size - 1)
                {
                    ++perm[0];
                    perm[size - 1] = perm[0] + 1;
                    isCorrect = true;
                }

                if (isCorrect)
                {                  
                    var tempArr = new bool[size];
                    for (int i = 0; i < size; ++i) tempArr[i] = true;

                    tempArr[perm[0] - 1] = false;
                    tempArr[perm[size - 1] - 1] = false;

                    for (int i = 1, j = 1; i < size - 1; ++i, ++j)
                    {
                        if (tempArr[j - 1]) perm[i] = j;
                        else if (tempArr[j++]) perm[i] = j;
                        else perm[i] = ++j;
                    }
                }
                return isCorrect;
            }

            int index2 = index1 + 1;

            while (index2 < size - 2 && perm[index2 + 1] > perm[index1]) ++index2;

            Swap(index1, index2);

            for (int i = index1 + 1; i < (index1 + size) / 2; ++i) Swap(i, size + index1 - i - 1);

            return true;
        }
    }
}
