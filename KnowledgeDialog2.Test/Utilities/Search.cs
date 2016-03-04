using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;    

namespace KnowledgeDialog2.Test.Utilities
{
    class Search
    {
        /// <summary>
        /// Searching for B instances.
        /// </summary>
        public static readonly WildcardTriplet ANY_is_B = WildcardTriplet.From(null, "is", "B");

        /// <summary>
        /// Searching for D instances.
        /// </summary>
        public static readonly WildcardTriplet ANY_is_D = WildcardTriplet.From(null, "is", "D");
    }
}
