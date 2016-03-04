using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

using KnowledgeDialog2.Inference.Triplet.Core;

namespace KnowledgeDialog2.Inference.Triplet.Rules
{
    public class AndStep : InferenceStep
    {
        private readonly TripletTreeReader _reader1;

        private readonly TripletTreeReader _reader2;

        internal AndStep(WildcardTriplet condition1, WildcardTriplet condition2, WildcardTriplet target, Context context)
            : base(target, context)
        {
            _reader1 = CreateRequirement(condition1);
            _reader2 = CreateRequirement(condition2);

            _reader1.AttachHandler(reader1_Handler);
            _reader2.AttachHandler(reader2_Handler);
        }

        /// <summary>
        /// Handler for reader 1.
        /// </summary>
        /// <param name="receivedTriplet">Triplet which was received from reader 1.</param>
        private void reader1_Handler(TripletTree receivedTriplet)
        {
            reader_Handler(receivedTriplet, _reader1);
        }

        /// <summary>
        /// Handler for reader 2.
        /// </summary>
        /// <param name="receivedTriplet">Triplet which was received from reader 2.</param>
        private void reader2_Handler(TripletTree receivedTriplet)
        {
            reader_Handler(receivedTriplet, _reader2);
        }

        /// <summary>
        /// Handler for received triplet on given reader.
        /// </summary>
        /// <param name="receivedTriplet">Triplet which was received.</param>
        /// <param name="handledReader">The reader to handle.</param>
        private void reader_Handler(TripletTree receivedTriplet, TripletTreeReader handledReader)
        {
            var otherReader = handledReader == _reader1 ? _reader2 : _reader1;
            var generateReverseOrder = handledReader == _reader2;

            var substitutions = handledReader.Condition.GetSubstitutionMapping(receivedTriplet);
            var otherConditionRequest = substitutions.Substitute(otherReader.Condition);

            foreach (var substitutedRequestedTriplet in otherReader.RequestSubstituted(otherConditionRequest))
            {
                var mapping = otherConditionRequest.GetSubstitutionMapping(substitutedRequestedTriplet);
                var substitutedReceivedTriplet = mapping.Substitute(receivedTriplet);

                if (generateReverseOrder)
                    Produce(TripletTree.From(substitutedRequestedTriplet, Predicate.And, substitutedReceivedTriplet));
                else
                    Produce(TripletTree.From(substitutedReceivedTriplet, Predicate.And, substitutedRequestedTriplet));
            }
        }

        internal static IEnumerable<InferenceStep> Provider(WildcardTriplet target, Context context)
        {
            if (!Predicate.And.Equals(target.SearchedPredicate))
                yield break;

            var condition1Tree = target.SearchedSubject as TripletTree;
            var condition2Tree = target.SearchedObject as TripletTree;

            if (condition1Tree == null || condition2Tree == null)
                yield break;

            yield return new AndStep(WildcardTriplet.Exact(condition1Tree), WildcardTriplet.Exact(condition2Tree), target, context);
        }
    }
}
