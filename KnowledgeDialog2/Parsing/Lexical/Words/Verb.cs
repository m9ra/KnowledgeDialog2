using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing.Lexical.Words
{
    class Verb : Word
    {
        /// <summary>
        /// Basic form of verb.
        /// </summary>
        public readonly string BasicForm;

        public Verb(string originalForm, string basicForm) : base(originalForm) {
            BasicForm = basicForm;
        }
    }
}
