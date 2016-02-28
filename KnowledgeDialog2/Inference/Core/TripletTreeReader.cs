using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.Inference.Core
{
    /// <summary>
    /// Event used for reporting of triplet receiving.
    /// </summary>
    /// <param name="triplet">The received triplet.</param>
    public delegate void TripletTreeReaderEvent(TripletTree triplet);

    public class TripletTreeReader
    {
        /// <summary>
        /// All triplets provided by the reader has to meet the condition.
        /// </summary>
        internal readonly WildcardTriplet Condition;

        /// <summary>
        /// Handler called for every received triplet.
        /// </summary>
        private readonly List<TripletTreeReaderEvent> _handlers = new List<TripletTreeReaderEvent>();

        private readonly HashSet<TripletTree> _availableTriplets = new HashSet<TripletTree>();


        internal TripletTreeReader(WildcardTriplet condition)
        {
            Condition = condition;
        }

        internal IEnumerable<TripletTree> RequestSubstituted(WildcardTriplet condition)
        {
            foreach (var triplet in _availableTriplets)
            {
                var substitution = condition.GetSatisfiingSubstitution(triplet);
                if (substitution != null)
                    yield return substitution;
            }
        }

        internal void AttachHandler(TripletTreeReaderEvent handler)
        {
            foreach (var triplet in _availableTriplets)
                handler(triplet);

            _handlers.Add(handler);
        }

        internal void Receive(TripletTree triplet)
        {
            _availableTriplets.Add(triplet);
            foreach (var handler in _handlers)
            {
                handler(triplet);
            }
        }
    }
}
