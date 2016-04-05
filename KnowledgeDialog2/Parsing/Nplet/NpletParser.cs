using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.Threading;

using KnowledgeDialog2.Database.Nplet;

using java.io;
using edu.stanford.nlp.util;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.trees;
using edu.stanford.nlp.process;
using edu.stanford.nlp.parser.lexparser;

namespace KnowledgeDialog2.Parsing.Nplet
{
    public class NpletParser
    {
        /// <summary>
        /// Factory used for tokenization.
        /// </summary>
        private readonly TokenizerFactory _tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");

        /// <summary>
        /// Standford parser.
        /// </summary>
        private readonly LexicalizedParser _parser;

        /// <summary>
        /// Converter from sentence to nplet.
        /// </summary>
        private readonly StanfordTreeConverter _converter = new StanfordTreeConverter();

        public NpletParser()
        {
            //---THIS IS UGLY FIX FOR STANDFORD PARSER---
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            _parser = LexicalizedParser.loadModel("../../../StanfordModels/englishPCFG.ser.gz");
            //-------------------------------------------
        }

        /// <summary>
        /// Parses given sentence to <see cref="NpletTree"/>.
        /// </summary>
        /// <param name="sentence">Sentence to be parsed.</param>
        /// <returns>The parsed <see cref="NpletTree"/>.</returns>
        public NpletTree Parse(string sentence)
        {
            var sentenceTree = parseSentence(sentence);
            return _converter.GetNplet(sentenceTree);
        }

        Tree parseSentence(string sentence)
        {
            var sentenceReader = new StringReader(sentence);
            var result = _tokenizerFactory.getTokenizer(sentenceReader).tokenize();
            sentenceReader.close();

            var tree = _parser.parse(result);
            return tree;
        }
    }
}
