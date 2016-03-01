using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.Utilities
{
    public static class ConsoleServices
    {
        /// <summary>
        /// Color for section boundaries.
        /// </summary>
        public static readonly ConsoleColor SectionBoundaries = ConsoleColor.Cyan;

        /// <summary>
        /// Color for prompt.
        /// </summary>
        public static readonly ConsoleColor PromptColor = ConsoleColor.Gray;

        /// <summary>
        /// Color for important text.
        /// </summary>
        public static readonly ConsoleColor ActiveColor = ConsoleColor.White;

        /// <summary>
        /// Color for operators.
        /// </summary>
        public static readonly ConsoleColor OperatorColor = ConsoleColor.Red;

        /// <summary>
        /// Color for captions.
        /// </summary>
        public static readonly ConsoleColor CaptionColor = ConsoleColor.Green;

        /// <summary>
        /// Color for regular text.
        /// </summary>
        public static readonly ConsoleColor InfoColor = ConsoleColor.Gray;

        /// <summary>
        /// Names of sections.
        /// </summary>
        private static readonly Stack<string> _sectionNames = new Stack<string>();

        /// <summary>
        /// Determine whether indentation is needed.
        /// </summary>
        private static bool _needIndent = true;

        /// <summary>
        /// Level of indentation.
        /// </summary>
        private static int _indentationLevel = 0;

        /// <summary>
        /// Begin new section with given name.
        /// </summary>
        /// <param name="sectionName">Name of section</param>
        public static void BeginSection(string sectionName)
        {
            PrintLine("".PadLeft(4, '=') + sectionName + "".PadLeft(4, '='), SectionBoundaries);
            ++_indentationLevel;

            _sectionNames.Push(sectionName);
        }

        /// <summary>
        /// End previous section.
        /// </summary>
        public static void EndSection()
        {
            --_indentationLevel;
            var borderLength = _sectionNames.Pop().Length + 8;
            PrintLine("".PadLeft(borderLength, '='), SectionBoundaries);
        }

        /// <summary>
        /// Prints prompt for user utterance.
        /// </summary>
        public static void PrintPrompt()
        {
            Print("utterance> ", PromptColor);
        }

        /// <summary>
        /// Prints line with data of given color.
        /// </summary>
        /// <param name="data">The data to be printed.</param>
        /// <param name="color">The color.</param>
        public static void PrintLine(object data, ConsoleColor color)
        {
            Print(data + Environment.NewLine, color);
            _needIndent = true;
        }

        /// <summary>
        /// Prints line with stringified triplets and given color.
        /// </summary>
        /// <param name="triplets">The triplets to be printed.</param>
        /// <param name="color">The color.</param>
        public static void PrintLine(IEnumerable<TripletTree> triplets, ConsoleColor color)
        {
            var data = string.Join(", ", triplets);
            PrintLine(data, color);
        }

        /// <summary>
        /// Prints data of given color.
        /// </summary>
        /// <param name="data">The data to be printed.</param>
        /// <param name="color">The color.</param>
        public static void Print(object data, ConsoleColor color)
        {
            var lastColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            if (_needIndent)
            {
                _needIndent = false;
                Console.Write("".PadLeft(_indentationLevel * 2) + data);
            }
            else
            {
                Console.Write(data);
            }
            Console.ForegroundColor = lastColor;
        }

        /// <summary>
        /// Prints an empty line.
        /// </summary>
        public static void PrintEmptyLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Reads line from user.
        /// </summary>
        /// <param name="color">Color of text displayed to user.</param>
        /// <returns>The read line.</returns>
        public static string ReadLine(ConsoleColor color)
        {
            var lastColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            var result = Console.ReadLine();
            Console.ForegroundColor = lastColor;

            return result;
        }

        /// <summary>
        /// Changes indentation.
        /// </summary>
        /// <param name="change">The change of indentation.</param>
        public static void Indent(int change)
        {
            _indentationLevel += change;
        }
    }
}
