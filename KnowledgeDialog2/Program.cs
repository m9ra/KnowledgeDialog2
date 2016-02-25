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
            var lexicalParser = new Parsing.Lexical.LexicalParser(args[0]);
            var expression = lexicalParser.Parse("anything which is made of snow is white");

            var tripletParser = new Parsing.Triplet.TripletParser();
            var result = tripletParser.Parse(expression).ToArray();

            Console.WriteLine(result[0]);
            Console.ReadKey();
        }
    }
}
