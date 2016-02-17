using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Utilities;

using KnowledgeDialog2.Parsing.Lexical;

namespace KnowledgeDialog2.Parsing.Triplet
{

    /// <summary>
    /// Parses <see cref="LexicalExpression"/> into <see cref="TripletTree"/> representation.
    /// </summary>
    class TripletParser
    {
        /// <summary>
        /// Rules for predicate creation ordered in descendend order - to avoid long/short ambiguity.
        /// </summary>
        private readonly List<PredicateRule> _orderedPredicateRules = new List<PredicateRule>();

        /// <summary>
        /// Rules for triplet creation ordered by priority.
        /// </summary>
        private readonly List<TripletRule> _orderedTripletRules = new List<TripletRule>();

        /// <summary>
        /// Index of word groups.
        /// </summary>
        private readonly Dictionary<string, WordGroup> _wordGroups = new Dictionary<string, WordGroup>();

        internal TripletParser()
        {
            RegisterWordGroup("variable_word")
                .Add("any", "anything", "anyone")
                .Add("every", "everything", "everyone")
                .Add("some", "something", "someone")
                ;

            //snowball is made of snow
            AddPredicatePattern("is [Verb] [Preposition]",
                c => c.Predicate(0, 1)
                );

            //has to 
            AddPredicatePattern("[Verb] [Preposition]",
                c => c.Predicate(0, 1)
                );

            //must
            AddPredicatePattern("[Verb]",
                c => c.Predicate(0)
                );


            //has to is same as must
            AddRule("#p1 #predicate #p2",
                c => c.Inform("p1", "predicate", "p2")
                );

            //snowball is made of snow
            AddRule("+subject #predicate +object",
                c => c.Inform("subject", "predicate", "object")
                );

            //any snowball is made of snow
            AddRule("{variable_word} +subject #predicate +object",
                c => c.Inform("subject", "predicate", "object")
                );

            //everyone needs air
            AddRule("{variable_word} #predicate +object",
                c => c.Inform(c.Variable(1), "predicate", "object")
                );

            //anything which is made of snow is white
            AddRule("{variable_word} [QuestionWord] #constraint_predicate +constraint_object #informed_predicate +informed_object",
                c => { throw new NotImplementedException(); }
                );

            //anything which is made of snow melts
            AddRule("{variable_word} [QuestionWord] #constraint_predicate +constraint_object #informed_predicate",
                c => { throw new NotImplementedException(); }
                );
        }

        /// <summary>
        /// Parses given expression to <see cref="TripletTree"/>.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <returns>The parsed triplets.</returns>
        public IEnumerable<TripletTree> Parse(LexicalExpression expression)
        {
            var lexicalGroups = createGroups(expression);
            var lexicalGroupsWithPredicates = groupPredicates(lexicalGroups);

            return getTriplets(lexicalGroupsWithPredicates);
        }

        /// <summary>
        /// Gets group of given name.
        /// </summary>
        /// <param name="groupName">The name of group.</param>
        /// <returns>The group.</returns>
        internal WordGroup GetGroup(string groupName)
        {
            return _wordGroups[groupName];
        }

        #region Parsing rule creation
        protected WordGroup RegisterWordGroup(string groupName)
        {
            var wordGroup = new WordGroup(groupName);
            _wordGroups.Add(groupName, wordGroup);
            return wordGroup;
        }

        protected void AddRule(string pattern, TripletFactory factory)
        {
            var rule = new TripletRule(pattern, factory, this);
            _orderedTripletRules.Add(rule);
        }

        protected void AddPredicatePattern(string pattern, PredicateFactory factory)
        {
            var rule = new PredicateRule(pattern, factory, this);
            _orderedPredicateRules.Add(rule);
            _orderedPredicateRules.Sort((r1, r2) => r2.Length - r1.Length);
        }
        #endregion

        #region Triplet parsing utilities

        private IEnumerable<TripletTree> getTriplets(IEnumerable<TripletWordGroup> groups)
        {
            var currentGroups = groups.ToArray();
            foreach (var tripletRule in _orderedTripletRules)
            {
                currentGroups = processGroups(tripletRule, currentGroups);
            }

            if (currentGroups.Length != 1)
                throw new NotImplementedException("Cannot parse into triplets");

            return new[] { currentGroups[0].AsTriplet() };
        }

        #endregion

        #region Predicate parsing utilities

        private IEnumerable<TripletWordGroup> createGroups(LexicalExpression expression)
        {
            var result = new List<TripletWordGroup>();
            foreach (var word in expression.Words)
            {
                result.Add(new TripletWordGroup(word));
            }

            return result;
        }

        private IEnumerable<TripletWordGroup> groupPredicates(IEnumerable<TripletWordGroup> groups)
        {
            var currentGroups = groups.ToArray();
            foreach (var predicateRule in _orderedPredicateRules)
            {
                currentGroups = processGroups(predicateRule, currentGroups);
            }

            return currentGroups;
        }

        #endregion

        private TripletWordGroup[] processGroups(WordGroupProcessor rule, TripletWordGroup[] groups)
        {
            var result = new List<TripletWordGroup>();
            for (var groupIndex = 0; groupIndex < groups.Length; ++groupIndex)
            {
                var match = rule.Match(groups, groupIndex);

                var group = groups[groupIndex];
                if (match != null)
                {
                    //matched groups will be replaced
                    group = match.ResultGroup;
                    groupIndex = groupIndex + match.Length - 1;
                }

                result.Add(group);
            }

            return result.ToArray();
        }
    }
}
