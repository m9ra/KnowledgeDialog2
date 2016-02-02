using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.MindModel;

namespace KnowledgeDialog2
{
    class Program
    {
        static void Main(string[] args)
        {
            var mind = new Mind();

            var tripletAB = TripletTree.Flat("A", "is", "B");
            var tripletCD = TripletTree.Flat("C", "is", "D");
            var tripletXY = TripletTree.Flat("X", "is", "Y");

            mind.AddAxiom(TripletTree.From(tripletAB, "then", tripletCD));
            mind.AddAxiom(TripletTree.From(tripletCD, "then", tripletXY));
            var falseAssertCD = mind.Holds(tripletCD);
            var falseAssertXY = mind.Holds(tripletXY);

            mind.AddAxiom(tripletAB);
            var trueAssertCD = mind.Holds(tripletCD);
            var trueAssertXY = mind.Holds(tripletXY);

            

            if (falseAssertCD || !trueAssertCD)
                throw new NotImplementedException("There is a BUG");

            if (falseAssertXY || !trueAssertXY)
                throw new NotImplementedException("There is a BUG");
        }
    }
}
