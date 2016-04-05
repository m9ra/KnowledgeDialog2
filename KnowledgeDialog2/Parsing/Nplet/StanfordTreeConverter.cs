using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using edu.stanford.nlp.trees;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Nplet;

namespace KnowledgeDialog2.Parsing.Nplet
{
    class StanfordTreeConverter
    {
        internal NpletTree GetNplet(Tree tree)
        {
            var nplet = convert(tree) as NpletTree;
            return nplet;
        }

        internal Predicate GetPredicate(Tree tree)
        {
            var predicate = convert(tree) as Predicate;
            return predicate;
        }

        private Entity convert(Tree tree)
        {
            var label = getLabel(tree);
            switch (label)
            {
                case "ROOT":
                    return convertRoot(tree);

                case "S":
                    return convertSentence(tree);

                case "VP":
                    return convertVP(tree);

                case "VBZ":
                    return convertVBZ(tree);

                case "VBG":
                    return convertVBG(tree);

                case "VB":
                    return convertVB(tree);

                case "TO":
                    return convertTO(tree);

                case "PP":
                    return convertPP(tree);

                case "NP":
                    return convertNP(tree);

                case "ADJP":
                    return convertADJP(tree);

                default:
                    throw new NotImplementedException();
            }
        }

        private NpletTree convertSentence(Tree sentence)
        {
            var predicate = convertChild(sentence, "VP") as NpletTree;
            var subject = convertChild(sentence, "NP") as Entity;

            if (predicate == null)
                throw new NotImplementedException("Sentence without a predicate");

            if (subject == null)
                return predicate;

            var edges = new[] { Edge.To(subject, Edge.Subject) };
            return NpletTree.From(predicate.Name, edges.Concat(predicate.Objects).ToArray());
        }

        private NpletTree convertVP(Tree predicateTree)
        {
            var objects = new List<Entity>();
            foreach (var child in getChildren(predicateTree))
            {
                var convertedChild = convert(child);
                objects.Add(convertedChild);
            }

            if (objects.Count == 0)
                //there are no objects
                throw new NotImplementedException();

            if (objects.Count == 1 && objects[0] is NpletTree)
                //simplification - pass the nplet higher
                return objects[0] as NpletTree;

            var freePredicates = objects.Where(e => e is Predicate).ToArray();
            if (freePredicates.Length > 1)
                throw new NotImplementedException("How to concat predicates?");

            var freePredicate = freePredicates.FirstOrDefault();
            if (freePredicate == null)
                throw new NotImplementedException("There are objects but no predicate.");

            objects.Remove(freePredicate);
            if (objects.Count == 1 && objects[0] is NpletTree)
            {
                var nplet = objects[0] as NpletTree;
                return NpletTree.From(freePredicate.Name + " " + nplet.Name, nplet.Objects.ToArray());
            }

            var edges = new List<Edge>();
            foreach (var obj in objects)
            {
                if (obj is Edge)
                {
                    //we have a named edge
                    edges.Add(obj as Edge);
                }
                else
                {
                    edges.Add(Edge.To(obj));
                }

            }

            return NpletTree.From(freePredicate.Name, edges.ToArray());
        }

        private IEnumerable<Tree> getObjects(Tree predicate)
        {
            var result = new List<Tree>();

            var children = getChildren(predicate);
            foreach (var child in children)
            {
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        private string getPredicateName(Tree predicate)
        {
            var children = getChildren(predicate);
            var name = "";
            foreach (var child in children)
            {
                var isVerbChild = getLabel(child).StartsWith("VB");
                if (isVerbChild)
                {
                    name += getSurface(child) + " ";
                }
            }
            return name.Trim();
        }

        private Entity convertRoot(Tree root)
        {
            var children = getChildren(root);
            if (children.Length != 1)
                throw new NotImplementedException("Root with non-one child");

            return convert(children[0]);
        }

        private Predicate convertVBZ(Tree vbzTree)
        {
            return createPredicate(getSurface(vbzTree));
        }

        private Predicate convertVBG(Tree vbgTree)
        {
            return createPredicate(getSurface(vbgTree));
        }

        private Predicate convertVB(Tree vbTree)
        {
            return createPredicate(getSurface(vbTree));
        }

        private Predicate convertTO(Tree toTree)
        {
            return createPredicate(getSurface(toTree));
        }

        private Entity convertNP(Tree npTree)
        {
            return createEntity(getSurface(npTree));
        }

        private Entity convertADJP(Tree npTree)
        {
            return createEntity(getSurface(npTree));
        }

        private Edge convertPP(Tree ppTree)
        {
            Tree prepositionTree = null;
            var objects = new List<Entity>();
            foreach (var child in getChildren(ppTree))
            {
                if (isPrepositionTree(child))
                {
                    if (prepositionTree != null)
                        throw new NotImplementedException("Multiple prepositions in PP");
                    prepositionTree = child;
                }
                else
                {
                    var obj = convert(child);
                    if (obj == null)
                        throw new NullReferenceException();
                    objects.Add(obj);
                }
            }

            if (prepositionTree == null)
                throw new NotImplementedException("No preposition tree found");

            if (objects.Count != 1)
                throw new NotImplementedException("Multiple objects for single preposition");

            return Edge.To(objects[0], getSurface(prepositionTree));
        }

        private bool isPrepositionTree(Tree tree)
        {
            switch (getLabel(tree))
            {
                case "TO":
                    return true;

                default:
                    return false;
            }
        }

        private Predicate createPredicate(string name)
        {
            return Predicate.From(name);
        }

        private Entity createEntity(string name)
        {
            return NamedEntity.From(name);
        }

        private object convertChild(Tree parent, string childLabel)
        {
            var children = getChildren(parent);
            var childrenToConvert = new List<Tree>();
            foreach (var child in children)
            {
                if (getLabel(child) == childLabel)
                {
                    childrenToConvert.Add(child);
                }
            }

            if (childrenToConvert.Count > 1)
                throw new NotImplementedException();

            if (childrenToConvert.Count == 1)
                return convert(childrenToConvert[0]);

            return null;
        }

        /// <summary>
        /// Gets root's label of given tree.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <returns>The label.</returns>
        private string getLabel(Tree tree)
        {
            return tree.label().value();
        }

        private string getSurface(Tree tree)
        {
            var queue = new Queue<Tree>();
            queue.Enqueue(tree);

            while (expansionStep(queue)) ;

            var stringBuilder = new StringBuilder();
            foreach (var terminal in queue)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(' ');

                stringBuilder.Append(terminal.value());
            }

            return stringBuilder.ToString();
        }

        private bool expansionStep(Queue<Tree> queue)
        {
            var hasChange = false;
            var count = queue.Count;
            for (var i = 0; i < count; ++i)
            {
                var expandedTree = queue.Dequeue();
                var children = getChildren(expandedTree);
                if (children.Length > 0)
                {
                    foreach (var child in children)
                    {
                        queue.Enqueue(child);
                        hasChange = true;
                    }
                }
                else
                {
                    queue.Enqueue(expandedTree);
                }
            }
            return hasChange;
        }

        private Tree[] getChildren(Tree tree)
        {
            var childrenList = tree.getChildrenAsList();
            var result = new List<Tree>();

            for (var i = 0; i < childrenList.size(); ++i)
            {
                var child = childrenList.get(i) as Tree;
                if (child == null)
                    throw new NullReferenceException("Child");
                result.Add(child);
            }


            return result.ToArray();
        }
    }
}
