using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.MindModel;

using KnowledgeDialog2.Parsing;
using KnowledgeDialog2.Parsing.Lexical;
using KnowledgeDialog2.Parsing.Lexical.Words;

using KnowledgeDialog2.Parsing.Triplet;

namespace KnowledgeDialog2
{
    class Program
    {
        static void Main(string[] args)
        {
            var testExpression = new LexicalExpression("snow is white", new Word[] { new Unknown("snow"), new Verb("is"), new Unknown("white") });
            var tripletParser = new Parsing.Triplet.TripletParser();

            var result = tripletParser.Parse(testExpression).ToArray();
            Console.WriteLine(result[0]);
        }
    }
}
