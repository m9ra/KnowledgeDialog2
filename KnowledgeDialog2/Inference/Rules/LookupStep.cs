using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Inference.Core;

namespace KnowledgeDialog2.Inference.Rules
{
    class LookupStep : InferenceStep
    {
        private readonly IEnumerator<TripletTree> _triplets;

        private readonly Context _context;

        private LookupStep(WildcardTriplet target, Context context) :
            base(target, context)
        {
            _context = context;
            _triplets = lazyFindTriplets().GetEnumerator();
        }

        internal static IEnumerable<InferenceStep> Provider(WildcardTriplet wildcard, Context context)
        {
            yield return new LookupStep(wildcard, context);
        }

        /// <inheritdoc/>
        internal override bool TryReportTriplet()
        {
            if (!_triplets.MoveNext())
                //there are no triplets available
                return false;

            _context.Report(_triplets.Current, Target);
            return true;
        }

        private IEnumerable<TripletTree> lazyFindTriplets()
        {
            foreach (var triplet in _context.FindSatisfiingSubstitutedRoots(Target))
            {
                yield return triplet;
            }
        }
    }
}
