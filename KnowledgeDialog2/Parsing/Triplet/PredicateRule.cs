using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Utilities;

namespace KnowledgeDialog2.Parsing.Triplet
{
    /// <summary>
    /// Factory for creating predicates based on given context.
    /// </summary>
    /// <param name="context">Context of parsing.</param>
    /// <returns>The created predicate.</returns>
    internal delegate Predicate PredicateFactory(ParsingContext context);

    class PredicateRule : WordGroupProcessor
    {
        /// <summary>
        /// Parser which created this rule.
        /// </summary>
        private readonly TripletParser _owner;

        /// <summary>
        /// Pattern of the rule.
        /// </summary>
        private readonly string _pattern;

        /// <summary>
        /// Factory for predicate creation.
        /// </summary>
        private readonly PredicateFactory _factory;

        /// <summary>
        /// Constraints composing the rule.
        /// </summary>
        private readonly PredicateRuleConstraint[] _constraints;

        /// <summary>
        /// Length of the rule.
        /// </summary>
        internal int Length { get { return _constraints.Length; } }

        internal PredicateRule(string pattern, PredicateFactory factory, TripletParser owner)
        {
            _pattern = pattern;
            _owner = owner;
            _factory = factory;
            _constraints = getPredicateConstraints(pattern);
        }

        private PredicateRuleConstraint[] getPredicateConstraints(string pattern)
        {
            var words = pattern.Split(' ');
            var constraints = new List<PredicateRuleConstraint>();
            foreach (var patternSplit in words)
            {
                var constraint = new PredicateRuleConstraint(patternSplit, _owner);
                constraints.Add(constraint);
            }

            return constraints.ToArray();
        }

        /// <inheritdoc/>
        internal override WordGroupMatch Match(TripletWordGroup[] groups, int groupIndex)
        {
            if (groupIndex + Length > groups.Length)
                //the input is not long enough
                return null;

            //predicate rules match one to one to groups
            var substitutedGroups = new List<TripletWordGroup>();
            for (var i = 0; i < _constraints.Length; ++i)
            {
                var constraint = _constraints[i];
                var group = groups[groupIndex + i];
                if (!constraint.Match(group))
                    //the constraint does'nt match
                    return null;

                if (constraint.CanBeSubstituted)
                    substitutedGroups.Add(group);
            }

            var context = new ParsingContext(substitutedGroups.ToArray());
            var predicate = _factory(context);

            var predicateGroup = new TripletWordGroup(predicate);
            return new WordGroupMatch(predicateGroup, Length);
        }
    }
}
