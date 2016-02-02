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
        internal override IEnumerable<TripletTree> InferNewSupportingTriplets(WildcardTriplet wildcard, InferenceContext context)
        {
            var parents = context.FindSubstitutedSubtreeParents(wildcard);

            //try to make inference step on triplets which possibly leads to answer for wildcard
            var result = new List<TripletTree>();
            foreach (var parent in parents.ToArray())
            {
                var supportingTriplet = makeInferenceStep(parent, context);
                if (supportingTriplet == null)
                    continue;
                result.Add(supportingTriplet);
            }

            return result;
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
    }
}
