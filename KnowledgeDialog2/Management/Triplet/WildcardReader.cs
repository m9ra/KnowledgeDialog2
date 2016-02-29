using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Inference;

namespace KnowledgeDialog2.Management.Triplet
{
    public abstract class WildcardReader
    {
        /// <summary>
        /// Determine whether there is an evidence for the wildcard.
        /// </summary>
        public abstract bool HasEvidence { get; }

        /// <summary>
        /// Determine whether there is an evidence for negation of the wildcard.
        /// </summary>
        public abstract bool HasNegativeEvidence { get; }

        /// <summary>
        /// Gets preconditions to make the wildcard 
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Precondition> GetPreconditions();
    }
}
