using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.MindModel.Inference
{
    public class AndRule : Rule
    {
        /// <summary>
        /// Predicate for and inference
        /// </summary>
        public static Predicate AndPredicate = Predicate.From("and");

        /// <inheritdoc/>
        internal override IEnumerable<TripletTree> FindSupportingTriplets(WildcardTriplet wildcard, InferenceContext context)
        {
            TripletTree condition1, condition2;
            if (!canInferWildcard(wildcard, out condition1, out condition2))
                yield break;

            //only supporting fact can be created 
            var condition1Holds = context.Holds(condition1);
            var condition2Holds = context.Holds(condition2);
            if (condition1Holds && condition2Holds)
                yield return TripletTree.From(condition1, AndPredicate, condition2);
        }

        /// <inheritdoc/>
        internal override IEnumerable<TripletTree> InferNewTriplets(IEnumerable<TripletTree> supportingTriplets, WildcardTriplet wildcard, InferenceContext context)
        {
            yield break;
        }

        /// <summary>
        /// Determine whether inference is possible for given wildcard.
        /// </summary>
        /// <param name="wildcard">Wildcard to infer.</param>
        /// <returns><c>true</c> if wildcard can be inferred by and, <c>false</c> otherwise.</returns>
        private bool canInferWildcard(WildcardTriplet wildcard, out TripletTree condition1, out TripletTree condition2)
        {
            condition1 = condition2 = null;
            if (!wildcard.SearchedPredicate.Equals(AndPredicate))
                //and can infer only triplets with and predicate
                return false;

            condition1 = wildcard.SearchedSubject as TripletTree;
            condition2 = wildcard.SearchedObject as TripletTree;
            if (condition1 == null || condition2 == null)
                //condition has to be in triplet form
                return false;

            return true;
        }

        /// <summary>
        /// Determine whether inference is possible for given wildcard.
        /// </summary>
        /// <param name="triplet">Triplet to infer.</param>
        /// <returns><c>true</c> if wildcard can be inferred by and, <c>false</c> otherwise.</returns>
        private bool canInfer(TripletTree triplet, out TripletTree condition1, out TripletTree condition2)
        {
            condition1 = condition2 = null;
            if (!triplet.Predicate.Equals(AndPredicate))
                //and can infer only triplets with and predicate
                return false;

            condition1 = triplet.Subject as TripletTree;
            condition2 = triplet.Object as TripletTree;
            if (condition1 == null || condition2 == null)
                //condition has to be in triplet form
                return false;

            return true;
        }
    }
}
