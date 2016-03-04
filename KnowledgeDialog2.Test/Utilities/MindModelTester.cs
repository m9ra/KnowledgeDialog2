using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Database.Triplet;

using KnowledgeDialog2.Inference.Triplet;   

namespace KnowledgeDialog2.Test.Utilities
{
    class MindModelTester
    {
        /// <summary>
        /// The tested mind model.
        /// </summary>
        private readonly InferenceEngine _mind = new InferenceEngine();

        /// <summary>
        /// Adds axiom into tested model.
        /// </summary>
        /// <param name="axiom">The axiom.</param>
        internal void AddAxiom(TripletTree axiom)
        {
            _mind.AddAxiom(axiom);
        }

        /// <summary>
        /// Asserts that given triplet doesn't hold in model.
        /// </summary>
        /// <param name="tripletTree">Asserted triplet</param>
        internal void AssertNotHolds(TripletTree tripletTree)
        {
            Assert.IsFalse(_mind.Holds(tripletTree), tripletTree.ToString());
        }

        /// <summary>
        /// Asserts that given triplet holds in model.
        /// </summary>
        /// <param name="triplet">Asserted triplet</param>
        internal void AssertHolds(TripletTree triplet)
        {
            Assert.IsTrue(_mind.Holds(triplet), triplet.ToString());
        }

        /// <summary>
        /// Asserts find on wildcard to given expected result.
        /// </summary>
        /// <param name="wildcardTriplet">The searching criterion.</param>
        /// <param name="expectedResult">Expected result.</param>
        internal void AssertFind(WildcardTriplet wildcardTriplet, params TripletTree[] expectedResult)
        {
            var actualResult = _mind.Find(wildcardTriplet).Take(10).ToArray();
            CollectionAssert.AreEquivalent(expectedResult, actualResult, "results of find");
        }
    }
}
