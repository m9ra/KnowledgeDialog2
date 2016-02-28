using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
