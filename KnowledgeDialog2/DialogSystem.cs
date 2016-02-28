using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2
{
    /// <summary>
    /// Representation of a complete dialog system.
    /// </summary>
    public abstract class DialogSystem
    {
        /// <summary>
        /// Process input given from user to an output.
        /// </summary>
        /// <param name="utterance">Utterance from a user.</param>
        /// <returns>Output for the user.</returns>
        protected abstract string input(string utterance);

        /// <summary>
        /// Process input given from user to an output.
        /// </summary>
        /// <param name="utterance">Utterance from a user.</param>
        /// <returns>Output for the user.</returns>
        public string Input(string utterance)
        {
            return input(utterance);
        }
    }
}
