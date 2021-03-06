﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Dialog;
using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;    

using KnowledgeDialog2.Utilities;

using KnowledgeDialog2.Parsing.Lexical;
using KnowledgeDialog2.Parsing.Triplet;
using KnowledgeDialog2.Management.Triplet;
using KnowledgeDialog2.Generation.Triplet;

namespace CurioChatBot
{
    class TripletCurioDialogSystem : DialogSystem
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

        /// <summary>
        /// Natural language generator.
        /// </summary>
        private readonly TripletNLG _nlGenerator;

        internal TripletCurioDialogSystem(string lexiconRoot)
        {
            _lexicalParser = new LexicalParser(lexiconRoot);
            _tripletParser = new TripletParser();
            _tripletManager = new CurioManager();
            _nlGenerator = new TripletNLG(TripletManager.Me);
        }

        /// <inheritdoc/>
        protected override string input(string utterance)
        {
            ConsoleServices.PrintLine("< " + utterance, ConsoleServices.ActiveColor);
            ConsoleServices.Indent(2);

            var inputTriplets = ParseToTriplets(utterance);
            ConsoleServices.PrintLine(inputTriplets, ConsoleServices.InfoColor);

            var responseTriplets = _tripletManager.AcceptInput(inputTriplets);
            var response = _nlGenerator.Generate(responseTriplets);

            ConsoleServices.Indent(-2);
            ConsoleServices.PrintLine("> " + response, ConsoleServices.ActiveColor);
            ConsoleServices.Indent(2);
            ConsoleServices.PrintLine(responseTriplets, ConsoleServices.InfoColor);
            ConsoleServices.Indent(-2);
            ConsoleServices.PrintEmptyLine();

            return response;
        }

        /// <summary>
        /// Parses given input to triplet representation.
        /// </summary>
        /// <param name="input">Input to be parsed.</param>
        /// <returns>The parse.</returns>
        internal IEnumerable<TripletTree> ParseToTriplets(string input)
        {
            var lexicalInput = _lexicalParser.Parse(input);
            var inputTriplets = _tripletParser.Parse(lexicalInput);
            return inputTriplets;
        }
    }
}
