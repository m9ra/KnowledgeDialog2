using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CurioChatBot.DataProcessing
{
    class TextToSentenceParser
    {
        /// <summary>
        /// Path to textfile which will be parsed into sentences.
        /// </summary>
        private readonly StreamReader _reader;

        internal TextToSentenceParser(string textFile)
        {
            _reader = new StreamReader(textFile);
        }

        /// <summary>
        /// Reads next sentence from the text file.
        /// </summary>
        /// <returns>The sentence.</returns>
        internal string NextSentence()
        {
            var sentenceBuilder = new StringBuilder();
            var isSentence = false;
            var lastChIsLetter = false;
            while (!_reader.EndOfStream)
            {
                var ch = (char)_reader.Read();
                var mayBeSentenceStart = !lastChIsLetter && char.IsUpper(ch);
                var isSentenceEnd = ".?!".Contains(ch);
                var isLetter = char.IsLetter(ch);
                var isPunctuation = char.IsPunctuation(ch);
                var isProhibitedChar = "\"':-_{}[]()".Contains(ch);

                //--next iteration setup
                lastChIsLetter = char.IsLetter(ch);
                //-----------------------

                if (sentenceBuilder.Length == 0 && !mayBeSentenceStart)
                    //we need start when builder is empty
                    continue;

                if (isLetter || isSentenceEnd || isPunctuation)
                    sentenceBuilder.Append(ch);

                if (char.IsWhiteSpace(ch) && sentenceBuilder[sentenceBuilder.Length - 1] != ' ')
                    sentenceBuilder.Append(' ');

                if (sentenceBuilder.Length > 100 || isProhibitedChar)
                    //sentence violated formating
                    sentenceBuilder.Clear();

                if (isSentenceEnd)
                {
                    var sentenceTooShort = sentenceBuilder.Length < 5 || !sentenceBuilder.ToString().Any(c => c == ' ');
                    var sentenceNotProperlyFormatted = sentenceTooShort || sentenceBuilder[sentenceBuilder.Length - 2] == ' ';

                    if (sentenceTooShort || sentenceNotProperlyFormatted)
                    {
                        //sentence does not meet requirements.
                        sentenceBuilder.Clear();
                    }
                    else
                    {
                        isSentence = true;
                        break;
                    }
                }
            }

            if (isSentence)
                return sentenceBuilder.ToString();
            else
                return null;
        }
    }
}
