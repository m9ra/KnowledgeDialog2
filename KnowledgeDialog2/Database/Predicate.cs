using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database
{
    public class Predicate : Entity
    {
        private Predicate(string name)
            : base(name)
        {
        }

        internal static Predicate From(string name)
        {
            return new Predicate(name);
        }
    }
}
