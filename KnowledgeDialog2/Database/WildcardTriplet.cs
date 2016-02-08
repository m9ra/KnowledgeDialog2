using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database
{
    public class WildcardTriplet
    {
        /// <summary>
        /// Subject which is searched. <c>null</c> if any subject is accepted.
        /// </summary>
        public readonly Entity SearchedSubject;

        /// <summary>
        /// Predicate which is searched. <c>null</c> if any subject is accepted.
        /// </summary>
        public readonly Predicate SearchedPredicate;

        /// <summary>
        /// Object which is searched. <c>null</c> if any subject is accepted.
        /// </summary>
        public readonly Entity SearchedObject;

        private WildcardTriplet(Entity searchedSubject, Predicate searchedPredicate, Entity searchedObject)
        {
            SearchedSubject = searchedSubject;
            SearchedPredicate = searchedPredicate;
            SearchedObject = searchedObject;
        }

        public static WildcardTriplet Exact(TripletTree triplet)
        {
            return new WildcardTriplet(triplet.Subject, triplet.Predicate, triplet.Object);
        }

        public static WildcardTriplet From(string searchedSubject, string searchedPredicate, string searchedObject)
        {
            return WildcardTriplet.From(null, Predicate.From(searchedPredicate), NamedEntity.From(searchedObject));
        }

        public static WildcardTriplet From(Entity searchedSubject, Predicate searchedPredicate, Entity searchedObject)
        {
            return new WildcardTriplet(searchedSubject, searchedPredicate, searchedObject);
        }

        internal bool IsSatisfiedBy(TripletTree triplet)
        {
            return (
                (SearchedSubject == null || SearchedSubject.Equals(triplet.Subject)) &&
                (SearchedPredicate == null || SearchedPredicate.Equals(triplet.Predicate)) &&
                (SearchedObject == null || SearchedObject.Equals(triplet.Object))
                );
        }

        /// <summary>
        /// Determine whether there exist satisfiing substituion in given triplet.
        /// </summary>
        /// <param name="triplet"></param>
        /// <returns></returns>
        internal bool IsSatisfiedBySubstitution(TripletTree triplet)
        {
            //test whether we have comaptible signature for substitution
            var hasCompatibleSignature = (
                (SearchedSubject == null || SearchedSubject.IsVariable || triplet.Subject.IsVariable || SearchedSubject.Equals(triplet.Subject)) &&
                (SearchedPredicate == null || SearchedPredicate.IsVariable || triplet.Predicate.IsVariable || SearchedPredicate.Equals(triplet.Predicate)) &&
                (SearchedObject == null || SearchedObject.IsVariable|| triplet.Object.IsVariable || SearchedObject.Equals(triplet.Object))
                );

            //test which variables are shared between triplet parts
            var sharedVariable_SP = triplet.Subject.IsVariable && triplet.Predicate.Equals(triplet.Subject);
            var sharedVariable_SO = triplet.Object.IsVariable && triplet.Predicate.Equals(triplet.Object);
            var sharedVariable_PO = triplet.Predicate.IsVariable && triplet.Object.Equals(triplet.Predicate);

            //test whether shared variables are not substituted by different entities
            return hasCompatibleSignature && (
                (!sharedVariable_SP || shareAble(SearchedSubject, SearchedPredicate)) &&
                (!sharedVariable_SO || shareAble(SearchedObject, SearchedPredicate)) &&
                (!sharedVariable_PO || shareAble(SearchedPredicate, SearchedObject))
               );
        }

        private bool shareAble(Entity searchedEntity1, Entity searchedEntity2)
        {
            if (searchedEntity1 == null || searchedEntity2 == null)
                //there is no constraint on both of entities
                return true;

            return searchedEntity1.Equals(searchedEntity2);
        }

        /// <summary>
        /// Determine whether wildcard can satisfy given triplet or any of its subtrees through substitution.
        /// </summary>
        /// <param name="triplet">Tested triplet.</param>
        /// <returns><c>true</c> if </returns>
        internal bool IsSatisfiedBySubtreeSubstitution(TripletTree triplet)
        {
            return triplet.Any(t => IsSatisfiedBySubstitution(t));
        }

        /// <summary>
        /// Gets satisfiing triplet substituted according to the wildcard.
        /// </summary>
        /// <param name="triplet">Triplet to substitute</param>
        /// <returns>The substituted triplet which satisfies the wildcard.</returns>
        internal TripletTree GetSatisfiingSubstitution(TripletTree triplet)
        {
            if (!IsSatisfiedBySubstitution(triplet))
                return null;

            var substitution = GetSubstitutionMapping(triplet);

            return substitution.Substitute(triplet);
        }

        /// <summary>
        /// Gest substitution mapping for the triplet.
        /// </summary>
        /// <param name="triplet">The triplet to substitute.</param>
        /// <returns>The mapping.</returns>
        internal SubstitutionMapping GetSubstitutionMapping(TripletTree triplet)
        {
            var substitution = new SubstitutionMapping();
            if (triplet.Subject.IsVariable && SearchedSubject != null)
                substitution.Map(triplet.Subject, SearchedSubject);

            if (triplet.Predicate.IsVariable && SearchedPredicate != null)
                substitution.Map(triplet.Predicate, SearchedPredicate);

            if (triplet.Object.IsVariable && SearchedObject != null)
                substitution.Map(triplet.Object, SearchedObject);

            return substitution;
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            var subjectStr = SearchedSubject == null ? "*" : SearchedSubject.ToString();
            var predicateStr = SearchedPredicate == null ? "*" : SearchedPredicate.ToString();
            var objectStr = SearchedObject == null ? "*" : SearchedObject.ToString();

            return string.Format("['{0}', '{1}', '{2}']", subjectStr, predicateStr, objectStr);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var o = obj as WildcardTriplet;
            if (o == null || GetType() != o.GetType())
                return false;

            //name fully distinguishes two wildcardtriplets
            return this.ToString() == o.ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
