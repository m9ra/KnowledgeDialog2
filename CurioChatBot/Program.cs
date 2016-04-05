using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Utilities;

using KnowledgeDialog2.Parsing.Lexical;
using KnowledgeDialog2.Parsing.Triplet;
using KnowledgeDialog2.Management.Triplet;

namespace CurioChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var dialogSystem = new NpletCurioDialogSystem();
            dialogSystem.Input("anything which is made of snow is white");
            dialogSystem.Input("anything which is made of frozen water is made of snow");
            dialogSystem.Input("is snowball white");
            dialogSystem.Input("is snowball white");
            dialogSystem.Input("is snowball white");
            dialogSystem.Input("snowball is made of frozen water");
            dialogSystem.Input("is snowball white");

            var sentenceProvider = new DataProcessing.TextToSentenceParser("data1.txt");

            ConsoleServices.PrintEmptyLine();
            ConsoleServices.PrintEmptyLine();

            while (true)
            {
                var sentence = sentenceProvider.NextSentence();
                if (sentence == null)
                    break;

                var nplet = dialogSystem.ParseToNplet(sentence);
                if (nplet == null)
                    //parsing was not successful
                    continue;
                ConsoleServices.PrintLine(sentence, ConsoleServices.ActiveColor);
                ConsoleServices.PrintLine(nplet, ConsoleServices.InfoColor);
                ConsoleServices.PrintEmptyLine();
            } 
            Console.ReadKey();
        }
    }
}
