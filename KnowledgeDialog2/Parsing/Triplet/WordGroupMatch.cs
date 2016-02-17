using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing.Triplet
{
    class WordGroupMatch
    {
        /// <summary>
        /// How many <see cref="WordGroup"/> groups is contained by the match.
        /// </summary>
        internal readonly int Length;

        /// <summary>
        /// The resulting triplet group made from matched <see cref="WordGroup"/>s.
        /// </summary>
        internal readonly TripletWordGroup ResultGroup;

        internal WordGroupMatch(TripletWordGroup resultGroup, int length)
        {
            Length = length;
            ResultGroup = resultGroup;
        }
    }
}
