using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

using KnowledgeDialog2.MindModel.Inference;

namespace KnowledgeDialog2.MindModel.Rules
{
    internal delegate IEnumerable<InferenceStep> InferenceStepProvider(WildcardTriplet wildcard, Context context);

    public abstract class InferenceStep
    {
        internal IEnumerable<WildcardTriplet> Requirements { get { return _requirements; } }

        protected readonly WildcardTriplet Target;

        private readonly Context _context;

        private readonly Queue<TripletTree> _producedTriplets = new Queue<TripletTree>();

        private readonly List<WildcardTriplet> _requirements = new List<WildcardTriplet>();

        internal InferenceStep(WildcardTriplet target, Context context)
        {
            Target = target;
            _context = context;
        }

        protected void Produce(TripletTree tripletTree)
        {
            _producedTriplets.Enqueue(tripletTree);
            _context.Report(tripletTree, Target);
        }

        protected TripletTreeReader CreateRequirement(WildcardTriplet wildcard)
        {
            _requirements.Add(wildcard);
            return _context.GetReader(wildcard);
        }

        internal virtual bool TryReportTriplet()
        {
            return false;
        }
    }
}
