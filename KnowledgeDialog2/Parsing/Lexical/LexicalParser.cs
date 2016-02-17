using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace KnowledgeDialog2.Parsing.Lexical
{
    /// <summary>
    /// Parses strings to words with lexical information.
    /// </summary>
    class LexicalParser
    {
        /// <summary>
        /// Words that are stored statically (no word changes are allowed).
        /// </summary>
        private readonly Dictionary<string, Word> _staticWords = new Dictionary<string, Word>();

        internal LexicalParser(string lexiconRoot)
        {
            var verbs = Path.Combine(lexiconRoot, "verbs.lex");
            var prepositions = Path.Combine(lexiconRoot, "prepositions.lex");

            requireFile(verbs);
            requireFile(prepositions);
        }

        /// <summary>
        /// Parses given expression into its lexical form.
        /// </summary>
        /// <param name="expression">The lexical form of expression.</param>
        /// <returns>The parsed form.</returns>
        internal LexicalExpression Parse(string expression)
        {
            var words = expression.Split(' ');
            var lexicalWords = new List<Word>();
            foreach (var word in words)
            {
                var lexicalWord = parse(word);
                lexicalWords.Add(lexicalWord);
            }

            return new LexicalExpression(expression, lexicalWords);
        }


        /// <summary>
        /// Throws exception when file on given path doesn't exist.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        private void requireFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File is required", path);
        }

        /// <summary>
        /// Parses given word.
        /// </summary>
        /// <param name="word">The word to parse.</param>
        /// <returns>The parsed word.</returns>
        private Word parse(string word)
        {
            Word result;
            if (_staticWords.TryGetValue(word, out result))
                //we can simply map the word on static representation.
                return result;

            throw new NotImplementedException();
        }
    }
}
