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
        /// Infers supporting triplets for given wildcard by using the context.
        /// </summary>
        /// <param name="wildcard">Wildcard which supporting triplets are required.</param>
        /// <param name="context">The context for inference.</param>
        /// <returns>Created supporting triplets.</returns>
        abstract internal IEnumerable<TripletTree> InferNewSupportingTriplets(WildcardTriplet wildcard, InferenceContext context);
    }
}
