using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database.Nplet;

namespace KnowledgeDialog2.Inference.Nplet.Core
{
    class QueryReader
    {
        /// <summary>
        /// Determine whether reader has next answer which can be produced.
        /// </summary>
        public bool HasNewAnswer { get { return _currentIndex < _source.AnswerCount; } }

        /// <summary>
        /// Source for the reader.
        /// </summary>
        private readonly NpletQueryNode _source;

        /// <summary>
        /// Index of curret answer.
        /// </summary>
        private int _currentIndex;

        internal QueryReader(NpletQueryNode source)
        {
            _source = source;
        }

        /// <summary>
        /// Gets next answer which is available for the reader.
        /// </summary>
        /// <returns>The answer if available, <c>null</c> otherwise.</returns>
        internal NpletTree GetNextAnswer()
        {
            if (!HasNewAnswer)
                return null;

            return _source.GetAnswer(_currentIndex++);
        }
    }
}
