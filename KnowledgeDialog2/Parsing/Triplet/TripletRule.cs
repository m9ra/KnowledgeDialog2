using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

namespace KnowledgeDialog2.Parsing.Triplet
{
    /// <summary>
    /// Factory for creating triplets based on given context.
    /// </summary>
    /// <param name="context">Context of parsing.</param>
    /// <returns>The created triplet.</returns>
    internal delegate TripletTree TripletFactory(ParsingContext context);

    class TripletRule : WordGroupProcessor
    {
        private readonly string _pattern;

        private readonly TripletParser _owner;

        private readonly TripletRuleConstraint[] _constraints;

        private readonly TripletFactory _factory;

        internal TripletRule(string pattern, TripletFactory factory, TripletParser owner)
        {
            _pattern = pattern;
            _owner = owner;
            _factory = factory;
            _constraints = createConstraints(pattern);
        }

        internal override WordGroupMatch Match(TripletWordGroup[] groups, int groupIndex)
        {
            //mapping of ids to matched groups
            var matchMapping = new Dictionary<string, IEnumerable<TripletWordGroup>>();
            var currentGroupIndex = groupIndex;

            //process all constraints
            for (var i = 0; i < _constraints.Length; ++i)
            {
                var constraint = _constraints[i];
                var nextConstraint = i + 1 >= _constraints.Length ? null : _constraints[i + 1];

                var matchedGroups = new List<TripletWordGroup>();
                matchMapping[constraint.Id] = matchedGroups;

                var isSatisfied = false;
                while (currentGroupIndex < groups.Length)
                {
                    var currentGroup = groups[currentGroupIndex];
                    var isMatch = constraint.Match(currentGroup);
                    var isNextMatch = nextConstraint != null && nextConstraint.Match(currentGroup);

                    if (isSatisfied && isNextMatch)
                        //we are greedy - matching as small groups as possible
                        //leting next constraint to match
                        break;

                    if (isMatch)
                    {
                        isSatisfied = true;
                        matchedGroups.Add(currentGroup);
                        ++currentGroupIndex;

                        if (!constraint.IsMultiMatch)
                            //we can match only one word to this constraint
                            break;
                    }
                    else
                    {
                        // current constraint cannot be further satisfied by current group
                        break;
                    }
                }

                if (!isSatisfied)
                    //constraint was not satisfied.
                    return null;
            }

            var context = new ParsingContext(matchMapping);
            var triplet = _factory(context);
            var tripletGroup = new TripletWordGroup(triplet);
            var match = new WordGroupMatch(tripletGroup, currentGroupIndex - groupIndex);
            return match;
        }

        private TripletRuleConstraint[] createConstraints(string pattern)
        {
            var words = pattern.Split(' ');
            var constraints = new List<TripletRuleConstraint>();

            foreach (var patternSplit in words)
            {
                var constraint = new TripletRuleConstraint(patternSplit, _owner);
                constraints.Add(constraint);
            }

            return constraints.ToArray();
        }
    }
}
