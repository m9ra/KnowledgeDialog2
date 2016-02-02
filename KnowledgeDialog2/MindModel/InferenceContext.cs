using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.MindModel
{
    class InferenceContext
    {
        /// <summary>
        /// Mind used for inference.
        /// </summary>
        private readonly Mind _mind;

        /// <summary>
        /// Actual supporting triplets.
        /// </summary>
        private readonly HashSet<TripletTree> _supportingTriplets = new HashSet<TripletTree>();

        /// <summary>
        /// Set of triplets that has been already requested.
        /// Is used for cyclic inference avoidance.
        /// </summary>
        private readonly HashSet<WildcardTriplet> _requestedTriplets = new HashSet<WildcardTriplet>();

        /// <summary>
        /// Stack of searched triplets.
        /// </summary>
        private readonly Stack<WildcardTriplet> _requestedTripletsStack = new Stack<WildcardTriplet>();

        internal InferenceContext(Mind mind)
        {
            _mind = mind;
        }

        /// <summary>
        /// Finds triplets according to given wildcard.
        /// Including the inference.
        /// </summary>
        /// <param name="wildcard">The wildcard for search.</param>
        /// <returns>Found triplets.</returns>
        internal IEnumerable<TripletTree> Find(WildcardTriplet wildcard)
        {
            if (!_requestedTriplets.Add(wildcard))
                //there is attempt for cyclic inference
                return new TripletTree[0];
            _requestedTripletsStack.Push(wildcard);

            initializeWithRootSupportingFacts(wildcard);

            var hasInferenceChange = true;
            while (hasInferenceChange)
            {
                hasInferenceChange = false;
                foreach (var rule in _mind.InferenceRules)
                {
                    var supportingTriplets = rule.InferNewSupportingTriplets(wildcard, this);
                    foreach (var supportingTriplet in supportingTriplets)
                    {
                        if (addSupportingTriplet(supportingTriplet))
                            hasInferenceChange = true;
                    }
                }
            }

            //we can release requested triplet
            _requestedTriplets.Remove(_requestedTripletsStack.Pop());

            //filter satisfiing triplets
            var result = new List<TripletTree>();
            foreach (var supportingTriplet in _supportingTriplets)
            {
                var substitution = wildcard.GetSatisfiingSubstitution(supportingTriplet);
                if (substitution != null)
                    result.Add(substitution);
            }
            return result;
        }

        /// <summary>
        /// Finds existing root supporting facts for given wildcard.
        /// </summary>
        /// <param name="wildcard">The searched wildcard.</param>
        private void initializeWithRootSupportingFacts(WildcardTriplet wildcard)
        {
            foreach (var rootTriplet in _mind.RootTriplets)
            {
                if (wildcard.IsSatisfiedBySubtreeSubstitution(rootTriplet))
                    //triplet is not compatible or satisfiable
                    addSupportingTriplet(rootTriplet);
            }
        }

        /// <summary>
        /// Adds supporting triplet to the context.
        /// </summary>
        /// <param name="triplet">The triplet to add.</param>
        /// <param name="supportedWildcard">Wildcard that is supported by triplet.</param>
        private bool addSupportingTriplet(TripletTree triplet)
        {
            return _supportingTriplets.Add(triplet);
        }

        internal IEnumerable<TripletTree> FindSubstitutedSubtreeParents(WildcardTriplet wildcard)
        {
            return _supportingTriplets;
        }

        internal bool Holds(TripletTree triplet)
        {
            return Find(WildcardTriplet.Exact(triplet)).Any();
        }
    }
}
