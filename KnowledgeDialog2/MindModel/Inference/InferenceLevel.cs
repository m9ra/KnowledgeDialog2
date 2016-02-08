using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

using KnowledgeDialog2.MindModel.Rules;

namespace KnowledgeDialog2.MindModel.Inference
{
    /// <summary>
    /// State of single 
    /// </summary>
    class InferenceLevel
    {
        /// <summary>
        /// Wildcard which inference step is represented here.
        /// </summary>
        internal WildcardTriplet Condition;

        /// <summary>
        /// Context of the inference step.
        /// </summary>
        private readonly Context _context;

        /// <summary>
        /// Index on step which will be read in next iteration.
        /// </summary>
        private int _currentStepIndex;

        /// <summary>
        /// The available steps for inference in current level.
        /// </summary>
        private readonly InferenceStep[] _steps;

        internal InferenceLevel(WildcardTriplet condition, IEnumerable<InferenceStep> steps, Context context)
        {
            Condition = condition;
            _context = context;
            _steps = steps.ToArray();
        }

        internal bool TryGenerateTriplet()
        {
            var end = _currentStepIndex + _steps.Length;
            for (; _currentStepIndex < end; ++_currentStepIndex)
            {
                var step = _steps[_currentStepIndex % _steps.Length];
                if (step.TryReportTriplet())
                    return true;
            }
            return false;
        }

        internal IEnumerable<WildcardTriplet> GetRequirements()
        {
            var requirements = new List<WildcardTriplet>();
            foreach (var step in _steps)
            {
                requirements.AddRange(step.Requirements);
            }

            return requirements;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("[InferenceLevel]{0}", Condition.ToString());
        }
    }
}
