using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

using KnowledgeDialog2.Inference.Triplet.Rules;

namespace KnowledgeDialog2.Inference.Triplet
{
    class Requirement
    {
        /// <summary>
        /// Target conditioned by the requirement.
        /// </summary>
        internal readonly WildcardTriplet Target;

        /// <summary>
        /// Rule which made the requirement.
        /// </summary>
        internal readonly InferenceStep InferenceRule;

        /// <summary>
        /// Inference condition.
        /// </summary>
        internal readonly IEnumerable<TripletTree> Condition;

        internal Requirement(WildcardTriplet target, InferenceStep inferenceRule, IEnumerable<TripletTree> condition)
        {
            Target = target;
            InferenceRule = inferenceRule;
            Condition = condition.ToArray();
        }
    }
}
