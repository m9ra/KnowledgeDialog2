﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Parsing.Lexical;
using KnowledgeDialog2.Parsing.Triplet;
using KnowledgeDialog2.Management.Triplet;

namespace CurioChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var dialogSystem = new CurioDialogSystem(args[0]);
            var response = dialogSystem.Input("anything which is made of snow is white");

            Console.WriteLine(response);
            Console.ReadKey();
        }
    }
}
