using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database.Nplet
{
    public class Edge
    {
        /// <summary>
        /// Nplet which owns the edge.
        /// </summary>
        public readonly NpletTree Owner;

        /// <summary>
        /// Target of the edge.
        /// </summary>
        public readonly Entity Target;

        /// <summary>
        /// Name of the edge.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Name of the then inference edge.
        /// </summary>
        public static readonly string Then = "then";

        internal static Edge TempVariable()
        {
            throw new NotImplementedException();
        }

        internal static Edge To(NpletTree query)
        {
            throw new NotImplementedException();
        }

        internal NpletTree TargetAsNplet { get { return Target as NpletTree; } }
    }
}
