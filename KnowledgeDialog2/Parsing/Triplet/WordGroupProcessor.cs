using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing.Triplet
{
    abstract class WordGroupProcessor
    {
        internal abstract WordGroupMatch Match(TripletWordGroup[] groups, int groupIndex);
    }
}
