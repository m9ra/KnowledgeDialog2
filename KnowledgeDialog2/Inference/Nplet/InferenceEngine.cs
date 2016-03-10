using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Nplet;

using KnowledgeDialog2.Inference.Nplet.Rules;

namespace KnowledgeDialog2.Inference.Nplet
{
    class InferenceEngine
    {
        /// <summary>
        /// Available inference rules.
        /// </summary>
        internal readonly IEnumerable<InferenceRule> Rules;

        internal InferenceEngine()
        {
            Rules = new[] { new GeneralizationRule() };
        }

        /// <summary>
        /// Finds triplets according to given wildcard.
        /// Including the inference.
        /// </summary>
        /// <param name="nplet">The wildcard for search.</param>
        /// <returns>Found nplets.</returns>
        public IEnumerable<NpletTree> Find(NpletTree nplet)
        {
            throw new NotImplementedException();
        }
    }
}
