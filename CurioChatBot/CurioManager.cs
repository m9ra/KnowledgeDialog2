using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

using KnowledgeDialog2.Inference.Triplet;
using KnowledgeDialog2.Management.Triplet;

namespace CurioChatBot
{
    class CurioManager : TripletManager
    {
        /// <summary>
        /// The engine that is used for inference.
        /// </summary>
        private readonly InferenceEngine _engine;

        internal CurioManager()
        {
            _engine = new InferenceEngine();
        }

        /// <inheritdoc/>
        protected override WildcardReader createWildcardReader(WildcardTriplet wildcard)
        {
            return new InferenceWildcardReader(_engine, wildcard);
        }

        /// <inheritdoc/>
        protected override bool acceptFact(TripletTree fact)
        {
            _engine.AddAxiom(fact);

            return true;
        }
    }
}
