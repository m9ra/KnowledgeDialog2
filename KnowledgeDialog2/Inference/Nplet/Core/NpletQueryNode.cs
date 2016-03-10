using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database.Nplet;

namespace KnowledgeDialog2.Inference.Nplet.Core
{
    class NpletQueryNode
    {
        /// <summary>
        /// Producers that was assigned.
        /// </summary>
        private readonly List<NpletProducerNode> _assignedProducers = new List<NpletProducerNode>();

        /// <summary>
        /// Producers that are assigned as subscribers.
        /// </summary>
        private readonly List<NpletProducerNode> _assignedSubscribers = new List<NpletProducerNode>();

        /// <summary>
        /// Answers that are available for current node.
        /// </summary>
        private readonly List<NpletTree> _availableAnswers = new List<NpletTree>();

        /// <summary>
        /// Index for available answers.
        /// </summary>
        private readonly HashSet<NpletTree> _availableAnswerIndex = new HashSet<NpletTree>();

        /// <summary>
        /// Query which answers are managed by this node.
        /// </summary>
        internal readonly NpletTree Query;

        /// <summary>
        /// Determine whether node was expanded.
        /// </summary>
        internal bool IsExpanded { get; private set; }

        /// <summary>
        /// Producers that produce nplets satisfiing the query.
        /// </summary>
        internal IEnumerable<NpletProducerNode> AssignedProducers { get { return _assignedProducers; } }

        /// <summary>
        /// Producers that are dependent on the query.
        /// </summary>
        internal IEnumerable<NpletProducerNode> AssignedSubscribers { get { return _assignedSubscribers; } }

        /// <summary>
        /// Count of available answers.
        /// </summary>
        internal int AnswerCount { get { return _availableAnswers.Count; } }

        internal NpletQueryNode(NpletTree query)
        {
            Query = query;
        }

        /// <summary>
        /// Adds answer to the query node.
        /// </summary>
        /// <param name="answer">Answer to be added.</param>
        /// <returns><c>true</c> if answer was added (it was new), <c>false</c> otherwise.</returns>
        internal bool AddAnswer(NpletTree answer)
        {
            if (!_availableAnswerIndex.Add(answer))
                return false;

            _availableAnswers.Add(answer);
            return true;
        }

        /// <summary>
        /// Assign producers to this query node.
        /// </summary>
        /// <param name="producers">Producers to be assigned.</param>
        internal void AssignProducers(IEnumerable<NpletProducerNode> producers)
        {
            if (IsExpanded)
                throw new NotSupportedException("Cannot be expanded twice.");

            IsExpanded = true;
            _assignedProducers.AddRange(producers);
        }

        /// <summary>
        /// Gets answer at given index.
        /// </summary>
        /// <param name="answerIndex">The zero based index of the answer.</param>
        /// <returns>The answer.</returns>
        internal NpletTree GetAnswer(int answerIndex)
        {
            return _availableAnswers[answerIndex];
        }
    }
}
