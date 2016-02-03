using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.MindModel.Inference
{
    public abstract class Rule
    {
        /// <summary>
        /// Finds supporting triplets for given wildcard by using the context.
        /// No inference is required here. 
        /// <remarks>Supporting triplet is a triplet which can cause inference of triplet which satisfies the wildcard by the rule (even though another (maybe unavailable) inference rules are required).</remarks>
        /// </summary>
        /// <param name="wildcard">Wildcard which supporting triplets are required.</param>
        /// <param name="context">The context for inference.</param>
        /// <returns>Found supporting triplets.</returns>
        abstract internal IEnumerable<TripletTree> FindSupportingTriplets(WildcardTriplet wildcard, InferenceContext context);

        /// <summary>
        /// Infers new triplets from given wildcard by using the context.
        /// No inference is required here. 
        /// </summary>
        /// <param name="wildcard">Wildcard which supporting triplets are required.</param>
        /// <param name="context">The context for inference.</param>
        /// <returns>Found supporting triplets.</returns>
        abstract internal IEnumerable<TripletTree> InferNewTriplets(IEnumerable<TripletTree> supportingTriplets, WildcardTriplet wildcard, InferenceContext context);
    }
}
