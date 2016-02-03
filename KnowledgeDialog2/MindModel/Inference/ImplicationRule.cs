using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.MindModel.Inference
{
    public class ImplicationRule : Rule
    {
        /// <summary>
        /// Predicate for implication inference
        /// </summary>
        public static Predicate ThenPredicate = Predicate.From("then");

        /// <inheritdoc/>
        internal override IEnumerable<TripletTree> FindSupportingTriplets(WildcardTriplet wildcard, InferenceContext context)
        {
            return context.FindSubstitutedSubtreeParents(wildcard);
        }

        /// <summary>
        /// Try to infer root triplet if possible.
        /// </summary>
        /// <param name="triplet">Triplet where inference starts.</param>
        /// <param name="context">Context where inference will be made.</param>
        /// <returns>The inferred tree.</returns>
        private TripletTree makeInferenceStep(TripletTree triplet, InferenceContext context)
        {
            if (!triplet.Predicate.Equals(ThenPredicate))
                //there is no possible inference
                return triplet;

            if (context.Holds(triplet.Subject as TripletTree))
                //implication works on triplets only
                return triplet.Object as TripletTree;

            //condition doesn't hold -  we cannot infer
            return null;
        }

        /// <inheritdoc/>
        internal override IEnumerable<TripletTree> InferNewTriplets(IEnumerable<TripletTree> supportingTriplets, WildcardTriplet wildcard, InferenceContext context)
        {
            foreach (var supportingTriplet in supportingTriplets)
            {
                var inferredTriplet = makeInferenceStep(supportingTriplet, context);
                if (inferredTriplet != null)
                    yield return inferredTriplet;
            }
        }
    }
}
