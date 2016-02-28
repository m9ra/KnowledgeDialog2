﻿using System;
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

        internal Database.TripletTree Triplet(object subjectIdentifier, object predicateIdentifier, object objectIdentifier)
        {
            var subject = getEntity(subjectIdentifier);
            var predicate = getPredicate(predicateIdentifier);
            var objectEntity = getEntity(objectIdentifier);

            return Database.TripletTree.From(subject, predicate, objectEntity);
        }

        internal Entity Variable(int index)
        {
            return NamedEntity.From("$" + index);
        }

        internal Predicate Predicate(params int[] tokenIndexes)
        {
            var predicateText = string.Join(" ", tokenIndexes.Select(i => _positionalGroups[i].TextSpan));
            return Database.Predicate.From(predicateText);
        }

        private Entity getEntity(object identifier)
        {
            if (identifier is Entity)
                return identifier as Entity;

            var identifierStr = identifier as string;
            if (identifierStr == null)
                throw new NotImplementedException();

            var groups = _namedGroups[identifierStr].ToArray();

            var entityName = string.Join(" ", groups.Select(g => g.TextSpan));
            var entity = NamedEntity.From(entityName);

            return entity;
        }

        private Predicate getPredicate(object identifier)
        {
            if (identifier is Entity)
                return identifier as Predicate;

            var identifierStr = identifier as string;
            if (identifierStr == null)
                throw new NotImplementedException();

            var groups = _namedGroups[identifierStr].ToArray();
            Predicate predicate = null;

            if (groups.Count() == 1)
                predicate = groups[0].RawGroup as Predicate;

            if (predicate == null)
                throw new KeyNotFoundException("Cannot get predicate on " + identifier);

            return predicate;
        }
    }
}
