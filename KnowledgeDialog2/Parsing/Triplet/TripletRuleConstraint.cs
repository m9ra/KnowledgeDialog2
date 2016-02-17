using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Utilities;

namespace KnowledgeDialog2.Parsing.Triplet
{
    class TripletRuleConstraint : RuleConstraint
    {
        /// <summary>
        /// Pattern for the constraint.
        /// </summary>
        private readonly string _pattern;

        /// <summary>
        /// Owner which created this rule.
        /// </summary>
        private readonly TripletParser _owner;

        /// <summary>
        /// Matcher of groups.
        /// </summary>
        private readonly Predicate<TripletWordGroup> _matcher;

        /// <summary>
        /// Id of the constraint inferred from the pattern.
        /// </summary>
        internal string Id;

        internal TripletRuleConstraint(string pattern, TripletParser owner)
        {
            _pattern = pattern;
            _owner = owner;
            
            var isWordGroup = pattern.StartsWith("{") && pattern.EndsWith("}");
            var isLexicalType = pattern.StartsWith("[") && pattern.EndsWith("]");
            var isPredicate = pattern.StartsWith("#");
            var isSubject = pattern.StartsWith("+");

            if (isWordGroup)
            {
                Id = pattern.WithoutParentheses();
                _matcher = createWordGroupMatcher(Id, _owner);
            }
            else if (isLexicalType)
            {
                Id = pattern.WithoutParentheses();
                _matcher = createLexicalTypeMatcher(Id);
            }
            else if (isPredicate)
            {
                Id = pattern.Substring(1);
                _matcher = createPredicateMatcher();
            }
            else if (isSubject)
            {
                Id = pattern.Substring(1);
                _matcher = createSubjectMatcher();
            }
            else
            {
                Id = ".word-" + pattern;
                _matcher = createExactWordMatcher(pattern);
            }
        }

        internal bool Match(TripletWordGroup group)
        {
            return _matcher(group);
        }
    }
}
