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
        /// Representation of beeing sure.
        /// </summary>
        public static readonly Entity Sure = NamedEntity.From("sure");

        /// <summary>
        /// Predicate describing that information is known already.
        /// </summary>
        public static readonly Predicate KnowAlready = Predicate.From("know already");

        /// <summary>
        /// Prediate describing that the information is still known.
        /// </summary>
        public static readonly Predicate StillKnow = Predicate.From("still know");

        /// <summary>
        /// Predicate describing that the information is still not known.
        /// </summary>
        public static readonly Predicate StillDontKnow = StillKnow.Negation;

        /// <summary>
        /// Predicate describing knowing.
        /// </summary>
        public static readonly Predicate Know = Predicate.From("know");

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
        /// Unanswered questions that the user had.
        /// </summary>
        private readonly List<TripletTree> _pendingQuestions = new List<TripletTree>();

        /// <summary>
        /// Preconditions that has been already asked.
        /// </summary>
        private readonly HashSet<TripletTree> _askedPreconditions = new HashSet<TripletTree>();

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
            var explicitAnswer = getAnswerWithQuestionConfirmation(reader, question);
            if (explicitAnswer != null)
                //we have answer
                return new[] { explicitAnswer };

            //otherwise we don't know
            var isKnownQuestion = _pendingQuestions.Contains(question);
            if (!isKnownQuestion)
                //the question has not been asked during this conversation
                _pendingQuestions.Add(question);

            //try to request additional information for question answering.
            var bestPrecondition = findBestPrecondition(reader);
            if (bestPrecondition == null)
            {
                //we don't have precondition to ask
                if (isKnownQuestion)
                    return triplet(Me, StillDontKnow, question);
                else
                    return triplet(Me, Know.Negation, question);
            }
            else
            {
                _askedPreconditions.Add(bestPrecondition);

                return
                    triplet(Me, Predicate.Is.Negation, Sure).Concat(
                    triplet(Entity.Question, Predicate.About, bestPrecondition)
                    );
            }
        }

        /// <summary>
        /// Finds best precondition which can be presented to the user according to question
        /// in the reader.
        /// </summary>
        /// <param name="reader">Reader which request is searched.</param>
        /// <returns>The request if any, <c>null</c> otherwise.</returns>
        private TripletTree findBestPrecondition(WildcardReader reader)
        {
            var preconditions = reader.GetPreconditions().Take(100).ToList();

            preconditions.Sort((a, b) => a.Score.CompareTo(b.Score));
            var bestCondition = preconditions.Where(p => !_askedPreconditions.Contains(toTriplet(p.Wildcard))).LastOrDefault();

            if (bestCondition == null)
                return null;

            return toTriplet(bestCondition.Wildcard);
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
                return triplet(Me, KnowAlready, fact);
            }
            else
            {
                if (reader.HasNegativeEvidence)
                    throw new NotImplementedException("inconsistency");

                acceptFact(fact);

                var clarifiedAnswer = getClarifiedAnswers().FirstOrDefault();
                if (clarifiedAnswer == null)
                {
                    return triplet(Me, Thank, You);
                }
                else
                {
                    return triplet(fact, Predicate.Then, clarifiedAnswer);

                }
            }
        }

        /// <summary>
        /// Gets answers for questions that has been clarified during time.
        /// Those questions are also removed from pending ones.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TripletTree> getClarifiedAnswers()
        {
            var result = new List<TripletTree>();
            foreach (var pendingQuestion in _pendingQuestions.ToArray())
            {
                var answer = getAnswerWithQuestionConfirmation(pendingQuestion);
                if (answer != null)
                {
                    result.Add(answer);
                    _pendingQuestions.Remove(pendingQuestion);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets answer containing the question confirmation.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>The answer if available, <c>null</c> otherwise.</returns>
        private TripletTree getAnswerWithQuestionConfirmation(TripletTree question)
        {
            var reader = createWildcardReader(question);
            return getAnswerWithQuestionConfirmation(reader, question);
        }

        /// <summary>
        /// Gets answer containing the question confirmation.
        /// </summary>
        /// <param name="reader">The reader containing question wildcard.</param>
        /// <param name="question">The question.</param>
        /// <returns>The answer if available, <c>null</c> otherwise.</returns>
        private TripletTree getAnswerWithQuestionConfirmation(WildcardReader reader, TripletTree question)
        {
            var isYesNoQuestion = Predicate.About.Equals(question.Predicate); //TODO better yes no resolving

            if (!isYesNoQuestion)
                throw new NotImplementedException();

            var questionObject = question.Object as TripletTree;
            if (reader.HasEvidence)
            {
                //there is evidence for the positive answer
                return questionObject;
            }
            else if (reader.HasNegativeEvidence)
            {
                return questionObject.Negation;
            }
            return null;
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
            WildcardTriplet wildcard;
            if (isQuestion(triplet))
            {
                var questionedFact = triplet.Object;
                //TODO placeholder replacement

                wildcard = WildcardTriplet.Exact(triplet.Object as TripletTree);
            }
            else
            {
                wildcard = WildcardTriplet.Exact(triplet);
            }


            return createWildcardReader(wildcard);
        }

        /// <summary>
        /// Creates triplet with given subject, predicate and object.
        /// </summary>
        /// <param name="subject">The subject of triplet.</param>
        /// <param name="predicate">The predicate of triplet.</param>
        /// <param name="obj">The object of triplet.</param>
        /// <returns>The created triplet wrapped into an array.</returns>
        private IEnumerable<TripletTree> triplet(Entity subject, Predicate predicate, Entity obj)
        {
            return new[] { TripletTree.From(subject, predicate, obj) };
        }

        /// <summary>
        /// Transforms wildcard to corresponding triplet.
        /// </summary>
        /// <param name="wildcard">The wildcard to be transformed.</param>
        /// <returns>The triplet.</returns>
        private TripletTree toTriplet(WildcardTriplet wildcard)
        {
            //TODO replace placeholders.
            return TripletTree.From(wildcard.SearchedSubject, wildcard.SearchedPredicate, wildcard.SearchedObject);
        }
    }
}
