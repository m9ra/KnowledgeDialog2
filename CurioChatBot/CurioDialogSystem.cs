using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2;

using KnowledgeDialog2.Parsing.Lexical;
using KnowledgeDialog2.Parsing.Triplet;
using KnowledgeDialog2.Management.Triplet;

namespace CurioChatBot
{
    class CurioDialogSystem : DialogSystem
    {
        /// <summary>
        /// Parser for lexical layer.
        /// </summary>
        private readonly LexicalParser _lexicalParser;

        /// <summary>
        /// Parser for triplet layer.
        /// </summary>
        private readonly TripletParser _tripletParser;

        /// <summary>
        /// Manager over the triplets.
        /// </summary>
        private readonly TripletManager _tripletManager;

        internal CurioDialogSystem(string lexiconRoot)
        {
            _lexicalParser = new LexicalParser(lexiconRoot);
            _tripletParser = new TripletParser();
            _tripletManager = new CurioManager();
        }

        /// <inheritdoc/>
        protected override string input(string utterance)
        {
            var lexicalExpression = _lexicalParser.Parse(utterance);
            var triplets = _tripletParser.Parse(lexicalExpression);
            var response = _tripletManager.AcceptInput(triplets);

            return string.Join(", ", response);
        }
    }
}
