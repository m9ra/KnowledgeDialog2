using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database.Nplet;

namespace KnowledgeDialog2.Inference.Nplet.Core
{
    class Context
    {
        /// <summary>
        /// Producers that are active now
        /// </summary>
        private readonly HashSet<NpletProducerNode> _activeProducers = new HashSet<NpletProducerNode>();

        /// <summary>
        /// Engine containing database and inference rules.
        /// </summary>
        private readonly InferenceEngine _engine;

        internal Context(InferenceEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Finds nplets satisfiing the query.
        /// </summary>
        /// <param name="query">The asked query.</param>
        /// <returns>The satisfiing nplets.</returns>
        internal IEnumerable<NpletTree> Find(NpletTree query)
        {
            var rootQueryNode = CreateQueryNode(query);
            var rootReader = CreateReader(rootQueryNode);

            var queriesToExpand = new Queue<NpletQueryNode>();
            queriesToExpand.Enqueue(rootQueryNode);

            _activeProducers.Clear();
            while (queriesToExpand.Count > 0 || _activeProducers.Count > 0)
            {
                //firstly we will expand newest dependencies
                expandQueries(queriesToExpand);

                //secondly we produce new answers
                produceAnswers();

                while (rootReader.HasNewAnswer)
                {
                    yield return rootReader.GetNextAnswer();
                }
            }
        }

        /// <summary>
        /// Creates reader for given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The reader.</returns>
        internal QueryReader CreateReader(NpletQueryNode query)
        {
            var reader = new QueryReader(query);

            return reader;
        }

        /// <summary>
        /// Creates query node for given nplet.
        /// </summary>
        /// <param name="nplet">The nplet.</param>
        /// <returns>The query node.</returns>
        internal NpletQueryNode CreateQueryNode(NpletTree nplet)
        {
            return new NpletQueryNode(nplet);
        }

        /// <summary>
        /// Fetches known nplets in form matching the query.
        /// </summary>
        /// <param name="query">Query used for matching of the nplets.</param>
        /// <returns>The nplets</returns>
        internal IEnumerable<NpletTree> Fetch(NpletTree query)
        {
            throw new NotImplementedException();
        }

        private void produceAnswers()
        {
            //producers that cannot 
            var processedProducers = new HashSet<NpletProducerNode>(_activeProducers);
            var producerQueue = new Queue<NpletProducerNode>(_activeProducers);

            //all producer has to activate itself
            //otherwise it will be removed from active ones.
            _activeProducers.Clear();

            while (producerQueue.Count > 0)
            {
                var activeProducer = producerQueue.Dequeue();
                var answer = activeProducer.ProduceAnswer();

                if (answer == null)
                    //the producer is not active
                    continue;

                _activeProducers.Add(activeProducer);

                var target = activeProducer.TargetQuery;
                if (!target.AddAnswer(answer))
                    //the answer is not new to the target
                    continue;

                //else we activate subscribers
                foreach (var subscriber in target.AssignedSubscribers)
                {
                    if (processedProducers.Add(subscriber))
                        //the subscriber has not been processed yet
                        producerQueue.Enqueue(subscriber);
                }
            }
        }

        private void expandQueries(Queue<NpletQueryNode> queriesToExpand)
        {
            foreach (var queryToExpand in queriesToExpand)
            {
                var producers = createProducers(queryToExpand);
                queryToExpand.AssignProducers(producers);

                foreach (var producer in producers)
                {
                    //we consider every new producer as activate
                    _activeProducers.Add(producer);
                    foreach (var dependency in producer.Dependencies)
                    {
                        if (dependency.IsExpanded)
                            //the dependency is already expanded
                            //because it has appeared as a dependency before the producer
                            continue;

                        //we have new dependency
                        queriesToExpand.Enqueue(dependency);
                    }
                }
            }
        }

        private IEnumerable<NpletProducerNode> createProducers(NpletQueryNode query)
        {
            var result = new List<NpletProducerNode>();
            foreach (var rule in _engine.Rules)
            {
                result.AddRange(rule.CreateProducers(query, this));
            }
            return result;
        }
    }
}
