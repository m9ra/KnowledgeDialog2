using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

using KnowledgeDialog2.MindModel.Inference;

namespace KnowledgeDialog2.MindModel.Rules
{
    class OrStep : InferenceStep
    {
        internal OrStep(Context context)
            : base(null, context)
        {
        }
    }
}
