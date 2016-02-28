using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Inference;
using KnowledgeDialog2.Management.Triplet;

namespace CurioChatBot
{
    class InferenceWildcardReader : WildcardReader
    {
        /// <summary>
        /// Engine for inferring evidences about the <see cref="_wildcard"/>.
        /// </summary>
        private readonly InferenceEngine _engine;

        /// <summary>
        /// Wildcard which is read.
        /// </summary>
        private readonly WildcardTriplet _wildcard;

        /// <inheritdoc/>
        public override bool HasEvidence
        {
            get { return _engine.Find(_wildcard).Any(); }
        }

        /// <inheritdoc/>
        public override bool HasNegativeEvidence
        {
            get
            {
                return _engine.Find(_wildcard.Negation).Any();
            }
        }

        internal InferenceWildcardReader(InferenceEngine engine, WildcardTriplet wildcard)
        {
            _engine = engine;
            _wildcard = wildcard;
        }
    }
}
