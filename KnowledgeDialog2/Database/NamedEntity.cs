using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database
{
    class NamedEntity : Entity
    {
        private NamedEntity(string name)
            : base(name)
        {
        }

        internal static NamedEntity From(string name)
        {
            return new NamedEntity(name);
        }
    }
}
