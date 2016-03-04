using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database.Triplet
{
    class SubstitutionMapping
    {
        /// <summary>
        /// Entity mapping.
        /// </summary>
        private Dictionary<Entity, Entity> _mapping = new Dictionary<Entity, Entity>();

        /// <summary>
        /// Sets mapping for given variable.
        /// </summary>
        /// <param name="variable">Variable which will be mapped.</param>
        /// <param name="substitution">The mapping entity.</param>
        internal void Map(Entity variable, Entity substitution)
        {
            if (!variable.IsVariable)
                throw new NotSupportedException("Can substitute only variables");

            if (substitution == null)
                throw new ArgumentNullException("Mapping");

            _mapping.Add(variable, substitution);
        }

        /// <summary>
        /// Recursively substitute given triplet.
        /// </summary>
        /// <param name="triplet">Triplet to substitute.</param>
        /// <returns>The substituted triplet.</returns>
        internal TripletTree Substitute(TripletTree triplet)
        {
            if (_mapping.Count == 0)
                //there is nothing to substitute
                return triplet;

            return TripletTree.From(
                substituted(triplet.Subject),
                substitutedPredicate(triplet.Predicate),
                substituted(triplet.Object)
                );
        }

        internal WildcardTriplet Substitute(WildcardTriplet wildcard)
        {
            if (_mapping.Count == 0)
                //there is nothing to substitute
                return wildcard;

            return WildcardTriplet.From(
                substituted(wildcard.SearchedSubject),
                substitutedPredicate(wildcard.SearchedPredicate),
                substituted(wildcard.SearchedObject)
                );
        }

        /// <summary>
        /// Returns substituted entity.
        /// </summary>
        /// <param name="entity">Entity to substitute.</param>
        /// <returns>The substitution.</returns>
        private Entity substituted(Entity entity)
        {
            var subtree = entity as TripletTree;
            if (subtree != null)
                //substitute whole tree
                return Substitute(subtree);

            //try to substitute the entity
            Entity result;
            if (!_mapping.TryGetValue(entity, out result))
                result = entity;

            return result;
        }

        /// <summary>
        /// Returns substituted predicate.
        /// </summary>
        /// <param name="predicate">Predicate to substitute.</param>
        /// <returns>The substitution.</returns>
        private Predicate substitutedPredicate(Predicate predicate)
        {
            Entity result;
            if (_mapping.TryGetValue(predicate, out result))
                predicate = (Predicate)result;

            return predicate;
        }

        /// <summary>
        /// Get all possible substitutions induced by subtrees, such that satisfies the wildcard.
        /// </summary>
        /// <param name="triplet">Triplet to substitution.</param>
        /// <param name="wildcard">Wildcard to satisfy.</param>
        /// <returns>The substitutions.</returns>
        internal static IEnumerable<TripletTree> GetSubtreeSubstitutions(TripletTree triplet, WildcardTriplet wildcard)
        {
            var substitutableSubtrees = new List<TripletTree>();
            triplet.Each(t =>
            {
                if (wildcard.IsSatisfiedBySubstitution(t))
                    substitutableSubtrees.Add(t);
            });

            foreach (var substitutableSubtree in substitutableSubtrees)
            {
                //TODO detection of infeasible mappings
                var mapping = wildcard.GetSubstitutionMapping(substitutableSubtree);
                yield return mapping.Substitute(triplet);
            }
        }
    }
}
