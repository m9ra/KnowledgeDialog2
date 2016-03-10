using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database.Nplet;

using KnowledgeDialog2.Inference.Nplet.Core;

namespace KnowledgeDialog2.Inference.Nplet.Rules
{
    class GeneralizationRule : InferenceRule
    {
        /// <inheritdoc/>
        internal override IEnumerable<NpletProducerNode> CreateProducers(NpletQueryNode query, Context context)
        {

            throw new NotImplementedException();
        }
    }
}
