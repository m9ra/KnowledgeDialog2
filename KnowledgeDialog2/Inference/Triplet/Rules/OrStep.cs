using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

using KnowledgeDialog2.Inference.Triplet.Core;

namespace KnowledgeDialog2.Inference.Triplet.Rules
{
    class OrStep : InferenceStep
    {
        internal OrStep(Context context)
            : base(null, context)
        {
        }
    }
}
