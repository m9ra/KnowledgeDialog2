using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database
{
    public class TripletTree : Entity
    {
        /// <summary>
        /// Subject of the triplet.
        /// </summary>
        internal readonly Entity Subject;

        /// <summary>
        /// Predicate of the triplet.
        /// </summary>
        internal readonly Predicate Predicate;

        /// <summary>
        /// Object of the triplet.
        /// </summary>
        internal readonly Entity Object;

        /// <summary>
        /// Determine whether root of tree contains variable.
        /// </summary>
        internal bool ContainsRootVariable { get { return Subject.IsVariable || Predicate.IsVariable || Object.IsVariable; } }

        /// <summary>
        /// Determine whether any triplet of tree contains variable.
        /// </summary>
        internal bool ContainsVariable { get { return Any(t => t.ContainsRootVariable); } }

        private TripletTree(Entity subject, Predicate predicate, Entity obj)
            : base(string.Format("('{0}', '{1}', '{2}')", subject.Name, predicate.Name, obj.Name))
        {
            Subject = subject;
            Predicate = predicate;
            Object = obj;
        }

        public static TripletTree Flat(string subjectName, string predicateName, string objectName)
        {
            return TripletTree.From(NamedEntity.From(subjectName), Predicate.From(predicateName), NamedEntity.From(objectName));
        }

        public static TripletTree From(Entity subject, string predicateName, Entity obj)
        {
            return TripletTree.From(subject, Predicate.From(predicateName), obj);
        }

        public static TripletTree From(Entity subject, Predicate predicate, Entity obj)
        {
            return new TripletTree(subject, predicate, obj);
        }

        /// <summary>
        /// Test whether any of triplets in subtree meets given condition.
        /// </summary>
        /// <param name="predicate">The predicate to run.</param>
        /// <returns><c>true</c> if any satisfiing triplet has been found, <c>false</c> otherwise.</returns>
        public bool Any(Predicate<TripletTree> predicate)
        {
            return predicate(this) ||
            tryRun(predicate, Subject) ||
            tryRun(predicate, Object);
        }

        /// <summary>
        /// Traverses all leaves of the tree with given action.
        /// </summary>
        /// <param name="action">Action to be runned on all leaves.</param>
        public void Each(Action<TripletTree> action)
        {
            action(this);
            tryRun(action, Subject);
            tryRun(action, Object);
        }

        #region Tree traversing utilities

        private bool tryRun(Predicate<TripletTree> predicate, Entity entity)
        {
            var tree = entity as TripletTree;
            if (tree == null)
                return false;

            return tree.Any(predicate);
        }

        private void tryRun(Action<TripletTree> action, Entity entity)
        {
            var tree = entity as TripletTree;
            if (tree == null)
                return;

            tree.Each(action);
        }

        #endregion

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var o = obj as TripletTree;
            if (o == null)
                return false;

            return Subject.Equals(o.Subject) && Predicate.Equals(o.Predicate) && Object.Equals(o.Object);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Subject.GetHashCode() + Predicate.GetHashCode() + Object.GetHashCode();
        }
    }
}
