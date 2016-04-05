using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database.Nplet
{
    public class Edge : Entity
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
        /// Name of the subject edge.
        /// </summary>
        public static readonly string Subject = ".subject";

        /// <summary>
        /// Name of the then inference edge.
        /// </summary>
        public static readonly string Then = "then";

        internal NpletTree TargetAsNplet { get { return Target as NpletTree; } }

        private Edge(string name, Entity target)
            : base(name)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            Target = target;
        }

        internal static Edge TempVariable()
        {
            throw new NotImplementedException();
        }

        internal static Edge To(Entity Target, string edgeName = null)
        {
            if (edgeName == null)
                edgeName = ".noname";

            return new Edge(edgeName, Target);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "--" + Name + "->" + Target.ToString();
        }
    }
}
