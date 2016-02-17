using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Utilities
{
    static class StringExtensions
    {
        public static string WithoutParentheses(this string stringWithParentheses)
        {
            return stringWithParentheses.Substring(1, stringWithParentheses.Length - 2);
        }
    }
}
