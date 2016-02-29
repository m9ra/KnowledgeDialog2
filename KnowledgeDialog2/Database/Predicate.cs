using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database
{
    public class Predicate : Entity
    {
        /// <summary>
        /// Predicate for and inference.
        /// </summary>
        public static readonly Predicate And = Predicate.From("and");

        /// <summary>
        /// Predicate for topic specification.
        /// </summary>
        public static readonly Predicate About = Predicate.From("about");

        /// <summary>
        /// Is predicate.
        /// </summary>
        public static readonly Predicate Is = Predicate.From("is");

        /// <summary>
        /// Predicate for implication inference.
        /// </summary>
        public static Predicate Then = Predicate.From("then");

        /// <summary>
        /// Negation of the predicate.
        /// </summary>
        public Predicate Negation
        {
            get
            {
                if (Name.StartsWith("!"))
                {
                    return Predicate.From(Name.Substring(1));
                }
                else
                {
                    return Predicate.From("!" + Name);
                }
            }
        }

        private Predicate(string name)
            : base(name)
        {
        }

        internal static Predicate From(string name)
        {
            return new Predicate(name);
        }
    }
}
