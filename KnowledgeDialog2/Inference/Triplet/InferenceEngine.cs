using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

using KnowledgeDialog2.Inference.Triplet.Rules;
using KnowledgeDialog2.Inference.Triplet.Core;

namespace KnowledgeDialog2.Inference.Triplet
{
    public class InferenceEngine
    {
        /// <summary>
        /// Available inference rules.
        /// </summary>
        private readonly List<InferenceStepProvider> _inferenceProviders = new List<InferenceStepProvider>();

        /// <summary>
        /// Facts that does not need inference.
        /// </summary>
        private readonly HashSet<TripletTree> _rootTriplets = new HashSet<TripletTree>();

        /// <summary>
        /// Root triplets.
        /// </summary>
        internal IEnumerable<TripletTree> RootTriplets { get { return _rootTriplets; } }

        /// <summary>
        /// Abilities available in current mind.
        /// </summary>
        internal IEnumerable<InferenceStepProvider> InferenceStepProviders { get { return _inferenceProviders; } }

        public InferenceEngine()
        {
            _inferenceProviders.Add(LookupStep.Provider);
            _inferenceProviders.Add(ImplicationStep.Provider);
            _inferenceProviders.Add(AndStep.Provider);
        }

        /// <summary>
        /// Receive given information.
        /// </summary>
        /// <param name="triplets">Received information.</param>
        public void Receive(IEnumerable<TripletTree> triplets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determine whether given information holds according to 
        /// actual mind state.
        /// </summary>
        /// <param name="triplet">The tested information</param>
        /// <returns><c>True</c> if triplet holds, <c>false</c> otherwise.</returns>
        public bool Holds(TripletTree triplet)
        {
            if (_rootTriplets.Contains(triplet))
                return true;

            return Find(WildcardTriplet.Exact(triplet)).Any();
        }

        /// <summary>
        /// Finds triplets according to given wildcard.
        /// Including the inference.
        /// </summary>
        /// <param name="wildcard">The wildcard for search.</param>
        /// <returns>Found triplets.</returns>
        public IEnumerable<TripletTree> Find(WildcardTriplet wildcard)
        {
            var inference = new Context(this, wildcard);
            return inference.Find();
        }


        /// <summary>
        /// Gets preconditions to given wildcard.
        /// </summary>
        /// <param name="wildcard">Wildcard which preconditions are requested.</param>
        /// <returns></returns>
        public IEnumerable<Precondition> Preconditions(WildcardTriplet wildcard)
        {
            var inference = new Context(this, wildcard);
            return inference.Preconditions();
        }

        /// <summary>
        /// Adds triplet as ground true (without any checks for absurdum)
        /// </summary>
        /// <param name="axiom">The axiom.</param>
        public void AddAxiom(TripletTree axiom)
        {
            registerTriplet(axiom);
        }

        /// <summary>
        /// Register given triplet into indexes.
        /// </summary>
        /// <param name="triplet">Triplet to </param>
        private void registerTriplet(TripletTree triplet)
        {
            _rootTriplets.Add(triplet);
        }
    }
}
