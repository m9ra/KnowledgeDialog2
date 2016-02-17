using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing
{
    class WordGroup
    {
        /// <summary>
        /// Words in the group.
        /// </summary>
        private readonly HashSet<string> _words = new HashSet<string>();

        /// <summary>
        /// Name of the group.
        /// </summary>
        private readonly string GroupName;

        internal WordGroup(string groupName)
        {
            GroupName = groupName;
        }

        internal WordGroup Add(params string[] words)
        {
            _words.UnionWith(words);
            return this;
        }

        internal bool Contains(string word)
        {
            return _words.Contains(word);
        }
    }
}
