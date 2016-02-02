﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.MindModel;

using KnowledgeDialog2.Test.Utilities;

namespace KnowledgeDialog2.Test
{
    [TestClass]
    public class MindModelTests
    {
        [TestMethod]
        public void VariableImplication_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.CD_implies_Var1B);
            mind.AddAxiom(Triplet.C_is_D);

            //inference should count with implication of variable
            mind.AssertHolds(Triplet.A_is_B);
            mind.AssertHolds(Triplet.C_is_B);
            mind.AssertHolds(Triplet.Var1_is_B);
        }

        [TestMethod]
        public void Variable_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.Var1_is_B);

            //we simply search for the triplets in database
            mind.AssertHolds(Triplet.A_is_B);
        }

        [TestMethod]
        public void Implication_EverySubjectSelection()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.A_is_B);
            mind.AddAxiom(Triplet.AB_implies_CD);
            mind.AddAxiom(Triplet.AB_implies_AD);

            //we simply search for the triplets in database
            mind.AssertFind(Search.ANY_is_D,
                Triplet.C_is_D,
                Triplet.A_is_D);
        }

        [TestMethod]
        public void NoInference_EverySubjectSelection()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.A_is_B);
            mind.AddAxiom(Triplet.C_is_B);

            //we simply search for the triplets in database
            mind.AssertFind(Search.ANY_is_B,
                Triplet.A_is_B,
                Triplet.C_is_B);
        }

        [TestMethod]
        public void NoInference_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.A_is_B);

            //we simply search for the triplet in database
            mind.AssertHolds(Triplet.A_is_B);
        }

        [TestMethod]
        public void Missing_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.AB_implies_CD);

            //there is no supporting fact for inference
            mind.AssertNotHolds(Triplet.A_is_B);
            mind.AssertNotHolds(Triplet.C_is_D);
        }

        [TestMethod]
        public void Implication_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.AB_implies_CD);
            mind.AddAxiom(Triplet.A_is_B);

            //there is supporting fact
            mind.AssertHolds(Triplet.C_is_D);
        }

        [TestMethod]
        public void ChainedImplication_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.AB_implies_CD);
            mind.AddAxiom(Triplet.CD_implies_XY);
            mind.AddAxiom(Triplet.A_is_B);

            //there is supporting fact
            mind.AssertHolds(Triplet.C_is_D);
            mind.AssertHolds(Triplet.X_is_Y);
        }
    }
}
