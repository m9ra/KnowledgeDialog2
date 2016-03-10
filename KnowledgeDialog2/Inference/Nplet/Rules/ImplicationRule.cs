using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database.Nplet;

using KnowledgeDialog2.Inference.Nplet.Core;

namespace KnowledgeDialog2.Inference.Nplet.Rules
{
    class ImplicationRule : InferenceRule
    {
        /// <inheritdoc/>
        internal override IEnumerable<NpletProducerNode> CreateProducers(NpletQueryNode query, Context context)
        {
            //creates query which will match to implication targeting the query
            var implicationQuery = NpletTree.From(Edge.Then, Edge.TempVariable(), Edge.To(query.Query));
            //fetches those implications from database
            var implications = context.Fetch(implicationQuery);

            var producers = new List<NpletProducerNode>();
            foreach (var implication in implications)
            {
                var condition = implication.GetEdge(0).TargetAsNplet;
                var producer = new NpletProducerNode(context, query, null, new[] { condition });

                producers.Add(producer);
            }

            return producers;
        }
    }
}
