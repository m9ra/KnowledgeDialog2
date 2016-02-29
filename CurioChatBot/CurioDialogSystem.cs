using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Dialog;
using KnowledgeDialog2.Utilities;

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
            ConsoleServices.PrintLine("> " + utterance, ConsoleServices.ActiveColor);
            ConsoleServices.Indent(1);

            var lexicalExpression = _lexicalParser.Parse(utterance);
            var triplets = _tripletParser.Parse(lexicalExpression);
            var response = _tripletManager.AcceptInput(triplets);
            var responseStr = string.Join(", ", response);

            ConsoleServices.PrintLine(string.Join(", ", triplets), ConsoleServices.InfoColor);
            ConsoleServices.Indent(-1);
            ConsoleServices.PrintLine("< " + responseStr, ConsoleServices.ActiveColor);
            ConsoleServices.PrintEmptyLine();

            return responseStr;
        }
    }
}
