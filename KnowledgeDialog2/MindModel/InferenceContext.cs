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

            initializeWithRootSupportingTriplets(wildcard);

            var newSupportingTriplets = new List<TripletTree>(_supportingTriplets);
            var currentSupportingTriplets = new List<TripletTree>();
            var hasInferenceChange = true;
            while (hasInferenceChange)
            {
                //swapp triplet lists
                var tmp = currentSupportingTriplets;
                currentSupportingTriplets = newSupportingTriplets;
                newSupportingTriplets = tmp;
                newSupportingTriplets.Clear();

                //infer new triplets from new supporting ones
                hasInferenceChange = false;
                foreach (var rule in _mind.InferenceRules)
                {
                    foreach (var supportingTriplet in rule.InferNewTriplets(currentSupportingTriplets, wildcard, this))
                    {
                        if (addSupportingTriplet(supportingTriplet))
                        {
                            newSupportingTriplets.Add(supportingTriplet);
                            hasInferenceChange = true;
                        }
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
        private void initializeWithRootSupportingTriplets(WildcardTriplet wildcard)
        {
            foreach (var rule in _mind.InferenceRules)
            {
                foreach (var supportingTriplet in rule.FindSupportingTriplets(wildcard, this))
                {
                    addSupportingTriplet(supportingTriplet);
                }
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

        /// <summary>
        /// Finds parents of triplets which can satisfy wildcard after substitution.
        /// </summary>
        /// <param name="wildcard">The searching wildcard.</param>
        /// <returns>The found result.</returns>
        internal IEnumerable<TripletTree> FindSubstitutedSubtreeParents(WildcardTriplet wildcard)
        {
            //TODO PERFORMANCE this should be done by using indexes
            foreach (var rootTriplet in _mind.RootTriplets)
            {
                if (wildcard.IsSatisfiedBySubtreeSubstitution(rootTriplet))
                    yield return rootTriplet;
            }
        }

        /// <summary>
        /// Determine whether the triplet holds according to actual knowledge.
        /// </summary>
        /// <param name="triplet">The tested triplet.</param>
        /// <returns><c>true</c> whether triplet can be inferred, <c>false</c> otherwise.</returns>
        internal bool Holds(TripletTree triplet)
        {
            return Find(WildcardTriplet.Exact(triplet)).Any();
        }
    }
}
