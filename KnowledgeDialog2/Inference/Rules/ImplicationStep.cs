using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

using KnowledgeDialog2.Inference.Core;

namespace KnowledgeDialog2.Inference.Rules
{
    public class ImplicationStep : InferenceStep
    {

        /// <summary>
        /// Reader for implication conditions.
        /// </summary>
        private readonly TripletTreeReader _conditionReader;

        /// <summary>
        /// The result of implication.
        /// </summary>
        private readonly TripletTree _implicationResult;

        internal ImplicationStep(WildcardTriplet implicationCondition, TripletTree implicationResult, WildcardTriplet target, Context context)
            : base(target, context)
        {
            _conditionReader = CreateRequirement(implicationCondition);
            _implicationResult = implicationResult;

            _conditionReader.AttachHandler(_handler_Condition);
        }

        private void _handler_Condition(TripletTree receivedTriplet)
        {
            var mapping = _conditionReader.Condition.GetSubstitutionMapping(receivedTriplet);
            var substitutedResult = mapping.Substitute(_implicationResult);
            Produce(substitutedResult);
        }

        internal static IEnumerable<InferenceStep> Provider(WildcardTriplet target, Context context)
        {
            foreach (var substitutedTree in context.FindSubstitutedSubtreeParents(target))
            {
                //find matching trees
                var thenTrees = new List<TripletTree>();
                substitutedTree.Each(t =>
                {
                    if (Predicate.Then.Equals(t.Predicate))
                        thenTrees.Add(t);
                });

                //create steps according to database
                foreach (var thenTree in thenTrees)
                {
                    var condition = thenTree.Subject as TripletTree;
                    var implicationResult = thenTree.Object as TripletTree;
                    if (condition != null && implicationResult != null && target.IsSatisfiedBy(thenTree.Object as TripletTree))
                    {
                        yield return new ImplicationStep(WildcardTriplet.Exact(condition), implicationResult, target, context);
                    }
                }
            }
        }
    }
}
