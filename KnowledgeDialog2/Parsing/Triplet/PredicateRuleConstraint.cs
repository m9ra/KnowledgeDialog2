using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Utilities;

namespace KnowledgeDialog2.Parsing.Triplet
{
    class PredicateRuleConstraint : RuleConstraint
    {
        /// <summary>
        /// Pattern for the constraint.
        /// </summary>
        private readonly string Pattern;

        /// <summary>
        /// Matcher of groups.
        /// </summary>
        private readonly Predicate<TripletWordGroup> _matcher;

        /// <summary>
        /// Determine whether constraint can be substituted.
        /// </summary>
        internal bool CanBeSubstituted;

        internal PredicateRuleConstraint(string pattern, TripletParser owner)
        {
            Pattern = pattern;

            var isWordGroup = pattern.StartsWith("{") && pattern.EndsWith("}");
            var isLexicalType = pattern.StartsWith("[") && pattern.EndsWith("]");

            if (isWordGroup)
            {
                CanBeSubstituted = true;
                _matcher = createWordGroupMatcher(pattern.WithoutParentheses(), owner);
            }
            else if (isLexicalType)
            {
                CanBeSubstituted = true;
                _matcher = createLexicalTypeMatcher(pattern.WithoutParentheses());
            }
            else
            {
                CanBeSubstituted = false;
                _matcher = createExactWordMatcher(pattern);
            }
        }

        internal bool Match(TripletWordGroup group)
        {
            return _matcher(group);
        }
    }
}
