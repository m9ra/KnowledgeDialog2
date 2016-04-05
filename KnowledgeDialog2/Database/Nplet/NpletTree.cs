using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database.Nplet
{
    public class NpletTree : Entity
    {
        /// <summary>
        /// Objects of the nplet (we consider subject to be object as well).
        /// </summary>
        public readonly IEnumerable<Edge> Objects;

        internal NpletTree(string name, IEnumerable<Edge> objects)
            : base(name)
        {
            Objects = objects.ToArray();
        }

        internal static NpletTree From(string npletName, params Edge[] edges)
        {
            return new NpletTree(npletName, edges);
        }

        internal Edge GetEdge(int edgeIndex)
        {
            throw new NotImplementedException();
        }
    }
}
