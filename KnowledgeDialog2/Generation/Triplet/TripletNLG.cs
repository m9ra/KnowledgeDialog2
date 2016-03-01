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
            var expressions = new List<string>();
            foreach (var triplet in triplets)
            {
                var expression = generate(triplet);
                expressions.Add(expression);
            }

            var joinedResult = string.Join(", ", expressions);
            var smoothedResult = smooth(joinedResult);
            var sentence = char.ToUpper(smoothedResult[0]) + smoothedResult.Substring(1) + ".";
            return sentence;
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
            replace(ref s, "I not", "I don't");
            replace(ref s, "we not", "we don't");
            replace(ref s, "you not", "you don't");

            //conventions
            replace(ref s, "question about", "whether");
            replace(ref s, "I thank", "Thank");

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
        /// Generates string representation of semantic represented by given triplet.
        /// </summary>
        /// <param name="triplet">The triplet.</param>
        /// <returns>The string representation.</returns>
        private string generate(TripletTree triplet)
        {
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
                return "not " + generate(predicate.Negation);

            //me dispatch
            if (_me.Equals(obj))
                return "I";

            //no special generation routines
            return obj.ToString();
        }

    }
}
