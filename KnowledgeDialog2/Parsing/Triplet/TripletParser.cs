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
                c => c.Predicate("p1", "predicate", "p2")
                );

            //snowball is made of snow
            AddRule("+subject #predicate +object",
                c => c.Predicate("subject", "predicate", "object")
                );

            //any snowball is made of snow
            AddRule("{variable_word} +subject #predicate +object",
                c => c.Predicate("subject", "predicate", "object")
                );

            //everyone needs air
            AddRule("{variable_word} #predicate +object",
                c => c.Predicate(c.Variable(1), "predicate", "object")
                );

            //anything which is made of snow is white
            AddRule("{variable_word} [QuestionWord] #constraint_predicate +constraint_object #informed_predicate +informed_object",
                c =>
                {
                    var constraint = c.Predicate(c.Variable(1), "constraint_predicate", "constraint_object");
                    var conclusion = c.Predicate(c.Variable(1), "informed_predicate", "informed_object");

                    return c.Predicate(constraint, Predicate.From("then"), conclusion);
                });

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
            var lexicalGroupsWithPredicates = parseGroups(lexicalGroups, _orderedPredicateRules);
            var groups = parseGroups(lexicalGroupsWithPredicates, _orderedTripletRules);

            var triplets = getTriplets(groups);
            return triplets;
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
            _orderedTripletRules.Insert(0, rule);
        }

        protected void AddPredicatePattern(string pattern, PredicateFactory factory)
        {
            var rule = new PredicateRule(pattern, factory, this);
            _orderedPredicateRules.Add(rule);
            _orderedPredicateRules.Sort((r1, r2) => r2.Length - r1.Length);
        }
        #endregion

        #region Parsing utilities

        private IEnumerable<TripletWordGroup> parseGroups<Processor>(IEnumerable<TripletWordGroup> groups, List<Processor> rules)
            where Processor : WordGroupProcessor
        {
            var currentGroups = groups.ToArray();
            var ruleIndex = 0;
            while (ruleIndex < rules.Count)
            {
                var tripletRule = rules[ruleIndex];
                var processedGroups = processGroups(tripletRule, currentGroups);
                if (processedGroups == null)
                {
                    //rule did not change anything
                    ++ruleIndex;
                }
                else
                {
                    //rule changed the groups
                    currentGroups = processedGroups;

                    //we will restart iteration to keep rules priority
                    ruleIndex = 0;
                }
            }

            return currentGroups;
        }

        private IEnumerable<TripletTree> getTriplets(IEnumerable<TripletWordGroup> groups)
        {
            if (groups.Count() != 1)
                throw new NotImplementedException("Cannot parse into triplets");

            return new[] { groups.First().AsTriplet() };
        }

        private IEnumerable<TripletWordGroup> createGroups(LexicalExpression expression)
        {
            var result = new List<TripletWordGroup>();
            foreach (var word in expression.Words)
            {
                result.Add(new TripletWordGroup(word));
            }

            return result;
        }

        #endregion

        private TripletWordGroup[] processGroups(WordGroupProcessor rule, TripletWordGroup[] groups)
        {
            var result = new List<TripletWordGroup>();
            for (var groupIndex = 0; groupIndex < groups.Length; ++groupIndex)
            {
                var match = rule.Match(groups, groupIndex);

                if (match != null)
                {
                    //matched groups will be replaced
                    result.Add(match.ResultGroup);
                    for (groupIndex = groupIndex + match.Length; groupIndex < groups.Length; ++groupIndex)
                    {
                        //remaining groups won't be changed in this iteration.
                        result.Add(groups[groupIndex]);
                    }

                    //stop the iteration to not break rule application priority
                    return result.ToArray();
                }

                var group = groups[groupIndex];
                result.Add(group);
            }

            //nothing has been matched
            return null;
        }
    }
}
