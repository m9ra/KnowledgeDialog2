using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Utilities;

using KnowledgeDialog2.Inference.Rules;

namespace KnowledgeDialog2.Inference.Core
{
    class Context
    {
        /// <summary>
        /// Mind used for inference.
        /// </summary>
        private readonly InferenceEngine _mind;

        /// <summary>
        /// Root state of the searched wildcard.
        /// </summary>
        private readonly InferenceLevel _rootLevel;

        /// <summary>
        /// Index of states.
        /// </summary>
        private readonly Dictionary<WildcardTriplet, InferenceLevel> _wildcardToLevel = new Dictionary<WildcardTriplet, InferenceLevel>();

        /// <summary>
        /// Readers attached to wildcards.
        /// </summary>
        private readonly Dictionary<WildcardTriplet, TripletTreeReader> _wildcardReaders = new Dictionary<WildcardTriplet, TripletTreeReader>();

        /// <summary>
        /// Queue of levels that can be expanded if there is nothing to read.
        /// </summary>
        private readonly Queue<InferenceLevel> _levelsToExpand = new Queue<InferenceLevel>();

        /// <summary>
        /// Queue of levels that can generate new triplets.
        /// </summary>
        private readonly Queue<InferenceLevel> _levelsToGenerate = new Queue<InferenceLevel>();

        /// <summary>
        /// Triplets that are waiting to be reported.
        /// </summary>
        private readonly Queue<TripletTree> _tripletsToReport = new Queue<TripletTree>();

        internal Context(InferenceEngine mind, WildcardTriplet rootWildcard)
        {
            _mind = mind;
            _rootLevel = getState(rootWildcard);
            _levelsToExpand.Enqueue(_rootLevel);
            _levelsToGenerate.Enqueue(_rootLevel);
        }

        /// <summary>
        /// Finds triplets for the <see cref="RootWildcard"/>.
        /// </summary>
        /// <returns>The triplets that has been found.</returns>
        internal IEnumerable<TripletTree> Find()
        {
            while (_levelsToGenerate.Count > 0 || _levelsToExpand.Count > 0)
            {
                //firstly we try to generate
                var generateCount = _levelsToGenerate.Count;
                for (var i = 0; i < generateCount; ++i)
                {
                    var levelToGenerate = _levelsToGenerate.Dequeue();
                    if (levelToGenerate.TryGenerateTriplet())
                        //we will try to generate next triplet next time
                        _levelsToGenerate.Enqueue(levelToGenerate);

                    while (_tripletsToReport.Count > 0)
                        //report all triplets that are currently available.
                        yield return _tripletsToReport.Dequeue();
                }

                //afterwards we can expand
                if (_levelsToExpand.Count == 0)
                    continue;

                var levelToExpand = _levelsToExpand.Dequeue();
                foreach (var expandedChild in expand(levelToExpand))
                {
                    _levelsToExpand.Enqueue(expandedChild);
                    _levelsToGenerate.Enqueue(expandedChild);
                }
            }
        }

        /// <summary>
        /// Reports inference of new triplet.
        /// </summary>
        /// <param name="triplet">The reported triplet.</param>
        /// <param name="inferenceLevel">Level which triplet was generated.</param>
        internal void Report(TripletTree triplet, WildcardTriplet wildcard)
        {
            var inferenceLevel = _wildcardToLevel[wildcard];
            if (inferenceLevel == _rootLevel)
            {
                _tripletsToReport.Enqueue(triplet);
                //we have found new triplet which satisfies the root condition.
                return;
            }

            var reader = GetReader(wildcard);
            reader.Receive(triplet);
        }

        internal TripletTreeReader GetReader(WildcardTriplet wildcard, TripletTreeReaderEvent handler = null)
        {
            TripletTreeReader reader;
            if (!_wildcardReaders.TryGetValue(wildcard, out reader))
                _wildcardReaders[wildcard] = reader = new TripletTreeReader(wildcard);

            if (handler != null)
                reader.AttachHandler(handler);
            return reader;
        }


        /// <summary>
        /// Finds parents of triplets which can satisfy wildcard after substitution.
        /// </summary>
        /// <param name="wildcard">The searching wildcard.</param>
        /// <returns>The found result.</returns>
        internal IEnumerable<TripletTree> FindSubstitutedSubtreeParents(WildcardTriplet wildcard)
        {
            //TODO PERFORMANCE this should be done by using indexes
            foreach (var rootTriplet in _mind.RootTriplets)
            {
                foreach (var substitution in SubstitutionMapping.GetSubtreeSubstitutions(rootTriplet, wildcard))
                {
                    yield return substitution;
                }
            }
        }

        internal IEnumerable<TripletTree> FindSatisfiingSubstitutedRoots(WildcardTriplet wildcard)
        {
            foreach (var rootTriplet in _mind.RootTriplets)
            {
                var substitution = wildcard.GetSatisfiingSubstitution(rootTriplet);
                if (substitution != null)
                    yield return substitution;
            }
        }

        /// <summary>
        /// Expands given inference level via back chaining.
        /// </summary>
        /// <param name="parent">The parent to be expanded.</param>
        /// <returns>The expansion.</returns>
        private IEnumerable<InferenceLevel> expand(InferenceLevel parent)
        {
            foreach (var requirement in parent.GetRequirements())
            {
                if (!_wildcardToLevel.ContainsKey(requirement))
                    yield return getState(requirement);
            }
        }

        /// <summary>
        /// Gets (or creates) state for inference of given wildcard.
        /// </summary>
        /// <param name="wildcard">The wildcard to infer.</param>
        /// <returns>The state.</returns>
        private InferenceLevel getState(WildcardTriplet wildcard)
        {
            InferenceLevel result;
            if (!_wildcardToLevel.TryGetValue(wildcard, out result))
                _wildcardToLevel[wildcard] = result = new InferenceLevel(wildcard, createSteps(wildcard), this);

            return result;
        }

        private IEnumerable<InferenceStep> createSteps(WildcardTriplet wildcard)
        {
            var result = new List<InferenceStep>();
            foreach (var stepProvider in _mind.InferenceStepProviders)
            {
                foreach (var step in stepProvider(wildcard, this))
                {
                    result.Add(step);
                }
            }

            return result;
        }
    }
}
