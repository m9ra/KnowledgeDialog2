using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.MindModel.Inference
{
    class AndRule:Rule
    {
        /// <inheritdoc/>
        internal override IEnumerable<TripletTree> InferNewSupportingTriplets(WildcardTriplet wildcard, InferenceContext context)
        {
            throw new NotImplementedException();
        }
    }
}
