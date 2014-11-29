using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamiltinianCompletion
{
    struct Completion<V>
    {
        public List<V> Cicle;
        public int AddOn;

        public Completion(List<V> cicle, int addOn)
        {
            Cicle = cicle;
            AddOn = addOn;
        }
    }
}
