using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Database
{
    public abstract class Entity
    {
        /// <summary>
        /// Entity for question representation.
        /// </summary>
        public static readonly Entity Question = NamedEntity.From("question");

        /// <summary>
        /// Distinguishing name of the entity.
        /// </summary>
        internal readonly string Name;

        /// <summary>
        /// Determine whether entity corresponds to a variable.
        /// </summary>
        public readonly bool IsVariable;

        protected Entity(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
            IsVariable = Name.StartsWith("$");
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var o = obj as Entity;
            if (o == null || GetType() != o.GetType())
                return false;

            //name fully distinguishes two entities
            return Name.Equals(o.Name);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
