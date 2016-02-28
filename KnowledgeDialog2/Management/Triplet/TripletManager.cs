using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Parsing.Triplet;

namespace KnowledgeDialog2.Management.Triplet
{
    public abstract class TripletManager
    {
        /// <summary>
        /// Representation of the system itself.
        /// </summary>
        public static readonly Entity Me = NamedEntity.From(".myself");

        /// <summary>
        /// Representation of the system counterpart which is it talking to.
        /// </summary>
        public static readonly Entity You = NamedEntity.From("you");

        /// <summary>
        /// Predicate describing that information is known already.
        /// </summary>
        public static readonly Predicate KnowAlready = Predicate.From("know already");

        /// <summary>
        /// Predicate describing thank.
        /// </summary>
        public static readonly Predicate Thank = Predicate.From("thank");

        /// <summary>
        /// Creates wildcard reader for given wildcard.
        /// </summary>
        /// <param name="wildcard">The wildcard to read.</param>
        /// <returns>The created reader.</returns>
        protected abstract WildcardReader createWildcardReader(WildcardTriplet wildcard);

        /// <summary>
        /// Accepts given fact as true.
        /// </summary>
        /// <param name="fact">The accepted fact.</param>
        /// <returns><c>True</c> whether the fact has been accepted, <c>false</c> otherwise.</returns>
        protected abstract bool acceptFact(TripletTree fact);

        /// <summary>
        /// Process triplets and provides according response.
        /// </summary>
        /// <param name="triplets">Input triplets.</param>
        /// <returns>The response.</returns>
        public IEnumerable<TripletTree> AcceptInput(IEnumerable<TripletTree> triplets)
        {
            var result = new List<TripletTree>();

            foreach (var triplet in triplets)
            {
                var response = acceptInput(triplet);
                result.AddRange(response);
            }

            return result;
        }

        /// <summary>
        /// Accepts given triplet as an input.
        /// </summary>
        /// <param name="triplet">The triplet to accept.</param>
        /// <returns>The response.</returns>
        private IEnumerable<TripletTree> acceptInput(TripletTree triplet)
        {
            if (isQuestion(triplet))
            {
                return answerQuestion(triplet);
            }
            else
            {
                return analyzeFact(triplet);
            }
        }

        /// <summary>
        /// Answers qiven question.
        /// </summary>
        /// <param name="question">The question to answer.</param>
        /// <returns>The response.</returns>
        private IEnumerable<TripletTree> answerQuestion(TripletTree question)
        {
            var reader = createWildcardReader(question);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles acception of the fact.
        /// </summary>
        /// <param name="fact">Fact to accept.</param>
        /// <returns>The response.</returns>
        private IEnumerable<TripletTree> analyzeFact(TripletTree fact)
        {
            var reader = createWildcardReader(fact);
            if (reader.HasEvidence)
            {
                //TODO try to infer new fact to implicit confirmation
                yield return TripletTree.From(Me, KnowAlready, fact);
            }
            else
            {
                if (reader.HasNegativeEvidence)
                    throw new NotImplementedException("inconsistency");

                acceptFact(fact);
                yield return TripletTree.From(Me, Thank, You);
            }
        }

        /// <summary>
        /// Determine whether triplet represents a question.
        /// </summary>
        /// <param name="triplet">The tested triplet.</param>
        /// <returns><c>True</c> whether question is represented by triplet,<c>false</c> otherwise.</returns>
        private bool isQuestion(TripletTree triplet)
        {
            return Entity.Question.Equals(triplet.Subject);
        }

        /// <summary>
        /// Creates wildcard reader for the triplet.
        /// </summary>
        /// <param name="triplet">Triplet defining the wildcard.</param>
        /// <returns>The created reader.</returns>
        private WildcardReader createWildcardReader(TripletTree triplet)
        {
            if (isQuestion(triplet))
                throw new NotImplementedException();

            var wildcard = WildcardTriplet.Exact(triplet);
            return createWildcardReader(wildcard);
        }
    }
}
