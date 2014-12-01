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

        public override string ToString()
        {
            var cicle = String.Join("--", Cicle);
            return String.Concat("Cicle: ", cicle, "\r\nAddOn: ", AddOn.ToString(), "\r\n");
        }
    }
}
