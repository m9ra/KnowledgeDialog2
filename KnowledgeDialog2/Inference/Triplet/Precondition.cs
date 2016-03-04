using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

namespace KnowledgeDialog2.Inference.Triplet
{
    public class Precondition
    {
        /// <summary>
        /// Score of the precondition - higher score, higher chance that the precondition will be answerable.
        /// </summary>
        public readonly double Score;

        /// <summary>
        /// Wildcard which is required as a precondition.
        /// </summary>
        public readonly WildcardTriplet Wildcard;

        public Precondition(WildcardTriplet wildcard, double score)
        {
            Wildcard = wildcard;
            Score = score;
        }
    }
}
