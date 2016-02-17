using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

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
            var currentGroup = groupIndex;

            //process all constraints
            for (var i = 0; i < _constraints.Length; ++i)
            {
                var constraint = _constraints[i];
                var nextConstraint = i + 1 < _constraints.Length ? _constraints[i + 1] : null;

                var matchedGroups = new List<TripletWordGroup>();
                matchMapping[constraint.Id] = matchedGroups;

                if (currentGroup > groups.Length)
                    //the constraint cannot be satisfied because there 
                    //are no more groups to match
                    return null;

                while (currentGroup < groups.Length)
                {
                    var group = groups[currentGroup];
                    var isMatch = constraint.Match(group);
                    var isNextConstraintMatch = nextConstraint != null && nextConstraint.Match(group);

                    if (isNextConstraintMatch)
                        //the matching algorithm is greedy
                        //we try to match as small groups as possible
                        break;

                    if (!isMatch)
                        //constraint is not satisfied
                        //and neither the NEXT CONSTRAINT 
                        return null;

                    ++currentGroup;
                    matchedGroups.Add(group);
                }
            }

            var context = new ParsingContext(matchMapping);
            var triplet = _factory(context);
            var tripletGroup = new TripletWordGroup(triplet);
            var match = new WordGroupMatch(tripletGroup, currentGroup - groupIndex);
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
