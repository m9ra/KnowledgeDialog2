using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using KnowledgeDialog2.Dialog;
using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Nplet;

using KnowledgeDialog2.Utilities;

using KnowledgeDialog2.Parsing.Nplet;

namespace CurioChatBot
{
    class NpletCurioDialogSystem : DialogSystem
    {
        /// <summary>
        /// Parser for input sentences.
        /// </summary>
        private readonly NpletParser _parser = new NpletParser();

        /// <inheritdoc/>
        protected override string input(string utterance)
        {
            ConsoleServices.PrintLine("< " + utterance, ConsoleServices.ActiveColor);
            ConsoleServices.Indent(2);

            var inputTriplets = ParseToNplet(utterance);
            ConsoleServices.PrintLine(inputTriplets, ConsoleServices.InfoColor);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses given input to nplet representation.
        /// </summary>
        /// <param name="input">Input to be parsed.</param>
        /// <returns>The parse.</returns>
        internal NpletTree ParseToNplet(string input)
        {
            return _parser.Parse(input);
        }
    }
}
