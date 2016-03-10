using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Inference.Nplet.Core;

namespace KnowledgeDialog2.Inference.Nplet.Rules
{
    abstract class InferenceRule
    {
        internal abstract IEnumerable<NpletProducerNode> CreateProducers(NpletQueryNode query, Context context);
    }
}
