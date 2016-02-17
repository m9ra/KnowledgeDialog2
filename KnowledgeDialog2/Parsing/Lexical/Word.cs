using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing.Lexical
{
    abstract class Word
    {
        /// <summary>
        /// Word as originaly appeared in the text.
        /// </summary>
        public readonly string OriginalForm;

        internal Word(string originalForm)
        {
            OriginalForm = originalForm;
        }
    }
}
