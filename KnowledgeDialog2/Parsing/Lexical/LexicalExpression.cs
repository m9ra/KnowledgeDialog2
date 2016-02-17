using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing.Lexical
{
    class LexicalExpression
    {
        /// <summary>
        /// Original expression from which words come from.
        /// </summary>
        private readonly string _originalExpression;

        /// <summary>
        /// Words with lexical information.
        /// </summary>
        private readonly Word[] _words;

        /// <summary>
        /// Words contained by the expression.
        /// </summary>
        internal IEnumerable<Word> Words { get { return _words; } }

        internal LexicalExpression(string originalExpression, IEnumerable<Word> words)
        {
            _originalExpression = originalExpression;
            _words= words.ToArray();
        }
    }
}
