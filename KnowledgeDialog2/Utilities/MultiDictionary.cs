using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeDialog2.Utilities
{
    class MultiDictionary<TKey, TValue>  // no (collection) base class
    {
        private Dictionary<TKey, List<TValue>> _data = new Dictionary<TKey, List<TValue>>();

        public void Add(TKey k, TValue v)
        {
            // can be a optimized a little with TryGetValue, this is for clarity
            if (_data.ContainsKey(k))
                _data[k].Add(v);
            else
                _data.Add(k, new List<TValue>() { v });
        }

        // more members

        internal IEnumerable<TValue> Get(TKey key)
        {
            List<TValue> result;
            if (_data.TryGetValue(key, out result))
                return result.ToArray();

            return Enumerable.Empty<TValue>();
        }
    }
}
