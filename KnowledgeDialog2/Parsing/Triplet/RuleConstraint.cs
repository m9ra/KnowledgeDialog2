using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Parsing.Triplet
{
    abstract class RuleConstraint
    {

        /// <summary>
        /// Determine whether the given group has the requested lexical type.
        /// </summary>
        /// <param name="lexicalType"></param>
        /// <param name="group"></param>
        /// <returns><c>true</c> if group has the given lexical type, <c>false</c> otherwise.</returns>
        internal bool HasLexicalType(string lexicalType, TripletWordGroup group)
        {
            var groupType = group.RawGroup.GetType().ToString();
            return groupType.EndsWith("." + lexicalType);
        }

        #region Matcher creation

        /// <summary>
        /// Creates group matcher that matches every word group from the group.
        /// </summary>
        /// <param name="lexicalType">Name of the word group.</param>
        /// <returns>The created matcher.</returns>
        protected Predicate<TripletWordGroup> createWordGroupMatcher(string groupName, TripletParser owner)
        {
            var wordGroup = owner.GetGroup(groupName);
            return g => wordGroup.Contains(g.TextSpan);
        }

        /// <summary>
        /// Creates group matcher that matches every word group with the lexical type.
        /// </summary>
        /// <param name="groupName">Name of the lexical type.</param>
        /// <returns>The created matcher.</returns>
        protected Predicate<TripletWordGroup> createLexicalTypeMatcher(string lexicalType)
        {
            return g => HasLexicalType(lexicalType, g);
        }

        /// <summary>
        /// Creates matcher for predicates.
        /// </summary>
        /// <returns>The created matcher.</returns>
        protected Predicate<TripletWordGroup> createPredicateMatcher()
        {
            return g => g.RawGroup is Database.Predicate;
        }


        /// <summary>
        /// Creates matcher for triplets.
        /// </summary>
        /// <returns>The created matcher.</returns>
        protected Predicate<TripletWordGroup> createTripletMatcher()
        {
            return g => g.RawGroup is Database.Triplet.TripletTree;
        }

        /// <summary>
        /// Creates matcher for subjects.
        /// </summary>
        /// <returns>The created matcher.</returns>
        protected Predicate<TripletWordGroup> createSubjectMatcher()
        {
            return g => HasLexicalType("Unknown", g);
        }

        /// <summary>
        /// Creates matcher that matches the exact word.
        /// </summary>
        /// <param name="word">The required word for matching.</param>
        /// <returns>The created matcher.</returns>
        protected Predicate<TripletWordGroup> createExactWordMatcher(string word)
        {
            return g => g.TextSpan == word;
        }

        #endregion
    }
}
