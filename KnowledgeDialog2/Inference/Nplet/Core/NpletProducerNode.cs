using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database.Nplet;

namespace KnowledgeDialog2.Inference.Nplet.Core
{
    internal delegate IEnumerable<NpletTree> NpletFactory(NpletProducerNode node);

    class NpletProducerNode
    {
        /// <summary>
        /// Query which answers are produced.
        /// </summary>
        internal readonly NpletQueryNode TargetQuery;

        /// <summary>
        /// Queries which answers are required for producing the target answers.
        /// </summary>
        internal readonly IEnumerable<NpletQueryNode> Dependencies;

        /// <summary>
        /// Readers which are used for reading of dependencies.
        /// </summary>
        private readonly QueryReader[] _dependencyReaders;

        /// <summary>
        /// Factory producing answer triplets.
        /// </summary>
        private readonly NpletFactory _factory;

        internal NpletProducerNode(Context context, NpletQueryNode target, NpletFactory factory, IEnumerable<NpletTree> dependencies)
        {
            TargetQuery = target;
            _factory = factory;

            Dependencies = (from nplet in dependencies select context.CreateQueryNode(nplet)).ToArray();
            _dependencyReaders = (from dependency in Dependencies select context.CreateReader(dependency)).ToArray();
        }

        internal NpletTree ProduceAnswer()
        {
            throw new NotImplementedException();
        }
    }
}
