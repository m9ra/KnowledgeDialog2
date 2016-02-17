using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.Parsing.Triplet
{
    class ParsingContext
    {
        /// <summary>
        /// Groups indexed by their positions.
        /// </summary>
        private readonly TripletWordGroup[] _positionalGroups;

        /// <summary>
        /// Groups indexed by their name.
        /// </summary>
        private Dictionary<string, IEnumerable<TripletWordGroup>> _namedGroups;

        internal ParsingContext(TripletWordGroup[] positionalGroups)
        {
            _positionalGroups = positionalGroups;
        }

        internal ParsingContext(Dictionary<string, IEnumerable<TripletWordGroup>> matchMapping)
        {
            _namedGroups = matchMapping;
        }

        internal Database.TripletTree Inform(string subjectIdentifier, string predicateIdentifier, string objectIdentifier)
        {
            var subject = getEntity(subjectIdentifier);
            var predicate = getPredicate(predicateIdentifier);
            var objectEntity = getEntity(objectIdentifier);

            return Database.TripletTree.From(subject, predicate, objectEntity);
        }

        internal string Variable(int p)
        {
            throw new NotImplementedException();
        }

        internal Predicate Predicate(params int[] tokenIndexes)
        {
            var predicateText = string.Join(" ", tokenIndexes.Select(i => _positionalGroups[i].TextSpan));
            return Database.Predicate.From(predicateText);
        }

        private Entity getEntity(string identifier)
        {
            var groups = _namedGroups[identifier].ToArray();

            var entityName = string.Join(" ",groups.Select(g => g.TextSpan));
            var entity = NamedEntity.From(entityName);

            return entity;
        }

        private Predicate getPredicate(string identifier)
        {
            var groups = _namedGroups[identifier].ToArray();
            Predicate predicate = null;

            if (groups.Count() == 1)
                predicate = groups[0].RawGroup as Predicate;

            if (predicate == null)
                throw new KeyNotFoundException("Cannot get predicate on " + identifier);

            return predicate;
        }
    }
}
