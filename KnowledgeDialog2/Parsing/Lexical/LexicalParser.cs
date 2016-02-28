using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;


using KnowledgeDialog2.Utilities;

using KnowledgeDialog2.Parsing.Lexical.Words;

namespace KnowledgeDialog2.Parsing.Lexical
{
    /// <summary>
    /// Parses strings to words with lexical information.
    /// </summary>
    public class LexicalParser
    {
        /// <summary>
        /// Words that are stored statically (no word changes are allowed).
        /// </summary>
        private readonly MultiDictionary<string, Word> _staticWords = new MultiDictionary<string, Word>();

        public LexicalParser(string lexiconRoot)
        {
            var verbs = Path.Combine(lexiconRoot, "verbs.lex");
            var prepositions = Path.Combine(lexiconRoot, "prepositions.lex");

            loadVerbs(verbs);
            loadPrepositions(prepositions);
            loadQuestionWords();
        }

        /// <summary>
        /// Parses given expression into its lexical form.
        /// </summary>
        /// <param name="expression">The lexical form of expression.</param>
        /// <returns>The parsed form.</returns>
        public LexicalExpression Parse(string expression)
        {
            var words = expression.Split(' ');
            var lexicalWords = new List<Word>();
            foreach (var word in words)
            {
                //TODO disambiguation
                var lexicalWord = parse(word).First();
                lexicalWords.Add(lexicalWord);
            }

            return new LexicalExpression(expression, lexicalWords);
        }

        /// <summary>
        /// Loads question words.
        /// </summary>
        private void loadQuestionWords()
        {
            foreach (var word in new[] { "who", "where", "when", "why", "what", "which", "how" })
            {
                _staticWords.Add(word, new QuestionWord(word));
            }
        }

        /// <summary>
        /// Loads prepositions from given lexicon file.
        /// </summary>
        /// <param name="path">The path to lexicon file.</param>
        private void loadPrepositions(string path)
        {
            requireFile(path);

            var prepositions = File.ReadLines(path);
            foreach (var preposition in prepositions)
            {
                _staticWords.Add(preposition, new Preposition(preposition));
            }
        }

        /// <summary>
        /// Load verbs from given lexicon file.
        /// </summary>
        /// <param name="path">The path to lexicon file.</param>
        private void loadVerbs(string path)
        {
            requireFile(path);

            registerVerb("be", "be");
            registerVerb("is", "be");
            registerVerb("are", "be");
            registerVerb("was", "be");
            registerVerb("were", "be");
            registerVerb("been", "be");
            registerVerb("being", "be");

            var lines = File.ReadLines(path);
            foreach (var line in lines)
            {
                var verbForms = line.Split(' ');
                var basicForm = verbForms[0];
                var pastForm = verbForms[1];
                var pastParticipleForm = verbForms[1];

                registerVerb(basicForm, basicForm);
                registerVerb(pastForm, basicForm);
                registerVerb(pastParticipleForm, basicForm);
            }
        }

        /// <summary>
        /// Registers given verb form.
        /// </summary>
        /// <param name="form">The form of verb.</param>
        /// <param name="basicForm">The basic form of verb.</param>
        private void registerVerb(string form, string basicForm)
        {
            var verb = new Verb(form, basicForm);
            _staticWords.Add(verb.OriginalForm, verb);
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
        private IEnumerable<Word> parse(string word)
        {
            var words = _staticWords.Get(word);
            if (words.Any())
                return words;

            return new[] { new Unknown(word) };
        }
    }
}
