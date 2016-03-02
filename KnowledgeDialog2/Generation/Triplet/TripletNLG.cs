using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Utilities;

namespace KnowledgeDialog2.Generation.Triplet
{
    /// <summary>
    /// Generator of natural language from triplets.
    /// </summary>
    public class TripletNLG
    {
        /// <summary>
        /// Entity representing the talking system.
        /// </summary>
        private readonly Entity _me;

        private static readonly HashSet<string> _auxiliaryVerbs = new HashSet<string>()
        {
            //primary auxiliary verbs
            "be", "is","am","are","was","were",
            "have","has","had",
            "do","does","did","done",

            //modal auxiliary verbs
            "will","would","can","could","may","might",
            "shall","should","must","ought","used"
        };

        public TripletNLG(Entity me)
        {
            _me = me;
        }

        /// <summary>
        /// Generates string representation of semantic represented by given triplets.
        /// </summary>
        /// <param name="triplets">The semantic representation.</param>
        /// <returns>The string representation.</returns>
        public string Generate(IEnumerable<TripletTree> triplets)
        {
            var sentences = new List<string>();
            foreach (var triplet in triplets)
            {
                var sentence = makeSentence(triplet);
                sentences.Add(sentence);
            }

            var joinedResult = string.Join(" ", sentences);
            return joinedResult;
        }

        /// <summary>
        /// Makes sentence from given triplet.
        /// </summary>
        /// <param name="triplet">Triplet which is used for triplet creation.</param>
        /// <returns>The sentence.</returns>
        private string makeSentence(TripletTree triplet)
        {
            var endMark = isQuestion(triplet) ? "?" : ".";

            var expression = generateSentence(triplet);
            expression = smooth(expression);
            expression = char.ToUpper(expression[0]) + expression.Substring(1) + endMark;

            return expression;
        }

        /// <summary>
        /// Smoothes given sentence (gramatically and conventionaly)
        /// </summary>
        /// <param name="sentence">The sentence to be smoothed.</param>
        /// <returns>The smoothed version of a sentence.</returns>
        private string smooth(string sentence)
        {
            var s = sentence;

            //grammar
            replace(ref s, "not is", "is not");
            replace(ref s, "I is", "I'm");
            replace(ref s, "we is", "We are");
            replace(ref s, "you is", "you are");
            replace(ref s, "they is", "they are");

            //conventions
            replace(ref s, "question about", "whether");
            replace(ref s, "I thank", "Thank");
            replace(ref s, "do not still", "still do not");
            replace(ref s, "do not", "don't");
            replace(ref s, "does not", "doesn't");

            return s;
        }

        /// <summary>
        /// Replaces all occurances of pattern by replacement in given sentence.
        /// </summary>
        /// <param name="sentence">Sentence to be replaced.</param>
        /// <param name="pattern">Pattern to be find.</param>
        /// <param name="replacement">Replacement.</param>
        /// <returns></returns>
        private void replace(ref string sentence, string pattern, string replacement)
        {
            var s = " " + sentence + " ";
            s = s.Replace(" " + pattern + " ", " " + replacement + " ", StringComparison.OrdinalIgnoreCase);
            sentence = s.Trim();
        }

        /// <summary>
        /// Generates string representation of semantic represented by given triplet as a standalone sentence without markers.
        /// </summary>
        /// <param name="triplet">The triplet.</param>
        /// <returns>The string representation.</returns>
        private string generateSentence(TripletTree triplet)
        {
            var tripletSubjectTree = triplet.Subject as TripletTree;
            var tripletObjectTree = triplet.Object as TripletTree;

            if (isQuestion(triplet))
            {
                var questionPredicatePart = generateQuestionPredicatePart(tripletObjectTree.Predicate);
                var dependentPredicatePart = generateDependentPredicatePart(tripletObjectTree.Predicate);

                if (questionPredicatePart != "")
                    questionPredicatePart += " ";

                if (dependentPredicatePart != "")
                    dependentPredicatePart += " ";

                return questionPredicatePart + generate(tripletObjectTree.Subject) + " " + dependentPredicatePart  + generate(tripletObjectTree.Object);
            }

            //otherwise we generate expression as usuall
            return generate(triplet);
        }

        /// <summary>
        /// Generates string representation of semantic represented by given triplet.
        /// </summary>
        /// <param name="triplet">The triplet.</param>
        /// <returns>The string representation.</returns>
        private string generate(TripletTree triplet)
        {
            var tripletSubjectTree = triplet.Subject as TripletTree;
            var tripletObjectTree = triplet.Object as TripletTree;

            var tripletRepresentation = generate(triplet.Subject) + " " + generate(triplet.Predicate) + " " + generate(triplet.Object);

            if (Predicate.Then.Equals(triplet.Predicate))
                tripletRepresentation = "when " + tripletRepresentation;

            return tripletRepresentation;
        }

        /// <summary>
        /// Generates string representation of semantic represented by given object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The string representation.</returns>
        private string generate(object obj)
        {
            //triplet dispatch
            var triplet = obj as TripletTree;
            if (triplet != null)
                return generate(triplet);

            //predicate dispatch
            var predicate = obj as Predicate;
            if (predicate != null && predicate.IsNegated)
            {
                var predicateText = generate(predicate.Negation);

                var auxiliaryVerb = findAuxiliary(predicate.Negation);
                if (auxiliaryVerb != null)
                    return predicateText.Replace(auxiliaryVerb, auxiliaryVerb + " not");

                return "do not " + predicateText;
            }

            //me dispatch
            if (_me.Equals(obj))
                return "I";

            //no special generation routines
            return obj.ToString();
        }

        /// <summary>
        /// Generates string representation predicate's part that can be
        /// used at begining of a question.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The generated representation.</returns>
        private string generateQuestionPredicatePart(Predicate predicate)
        {
            var auxiliaryVerb = findAuxiliary(predicate);

            if (auxiliaryVerb == null)
                return "do";

            //questions begins with auxilar verb if possible
            return auxiliaryVerb;
        }

        /// <summary>
        /// Generates string representation predicate's part that can be
        /// used at begining of a question.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The generated representation.</returns>
        private string generateDependentPredicatePart(Predicate predicate)
        {
            var predicateText = generate(predicate);
            var auxiliaryVerb = findAuxiliary(predicate);
            if (auxiliaryVerb == null)
                //whole predicate will be used
                return predicateText;

            return predicateText.Replace(auxiliaryVerb, "").Trim();
        }

        /// <summary>
        /// Gets auxiliary verb from given predicate if available.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The auxiliary verb in same for as in predicate, <c>null</c> if such verb is not present.</returns>
        private string findAuxiliary(Predicate predicate)
        {
            var text = generate(predicate);
            var words = text.Split(' ');

            foreach (var word in words)
            {
                if (_auxiliaryVerbs.Contains(word))
                    return word;
            }

            return null;
        }

        /// <summary>
        /// Determine whether given triplet describes a question.
        /// </summary>
        /// <param name="triplet">Triplet to be tested.</param>
        /// <returns><c>true</c> when triplet is question, <c>false</c> otherwise.</returns>
        private bool isQuestion(TripletTree triplet)
        {
            return Entity.Question.Equals(triplet.Subject) && triplet.Object as TripletTree != null;
        }
    }
}
