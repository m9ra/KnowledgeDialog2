using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

using KnowledgeDialog2.Parsing.Lexical;

namespace KnowledgeDialog2.Parsing.Triplet
{
    class TripletWordGroup
    {
        /// <summary>
        /// Span representing the group in textual form.
        /// </summary>
        public readonly string TextSpan;

        /// <summary>
        /// Raw representation of the group.
        /// </summary>
        internal readonly object RawGroup;

        internal TripletWordGroup(Word word)
        {
            RawGroup = word;
            TextSpan = word.OriginalForm;
        }

        internal TripletWordGroup(Predicate predicate)
        {
            RawGroup = predicate;
            TextSpan = predicate.Name;
        }

        public TripletWordGroup(TripletTree triplet)
        {
            // TODO: Complete member initialization
            TextSpan = triplet.Subject.Name + " " + triplet.Predicate.Name + " " + triplet.Object.Name;

            RawGroup = triplet;
        }

        internal TripletTree AsTriplet()
        {
            return RawGroup as TripletTree;
        }
    }
}
