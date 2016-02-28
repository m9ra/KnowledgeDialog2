using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using KnowledgeDialog2.Database;
using KnowledgeDialog2.Inference;

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
        public void AndImplication_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.AB_and_CD_implies_EF);
            mind.AddAxiom(Triplet.A_is_B);
            mind.AddAxiom(Triplet.C_is_D);

            //there is supporting fact
            mind.AssertHolds(Triplet.E_is_F);
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

        [TestMethod]
        public void ChainedAndImplication_SupportingTriplet()
        {
            var mind = new MindModelTester();

            mind.AddAxiom(Triplet.AB_and_CD_implies_EF);
            mind.AddAxiom(Triplet.EF_and_GH_implies_XY);
            mind.AddAxiom(Triplet.A_is_B);
            mind.AddAxiom(Triplet.C_is_D);
            mind.AddAxiom(Triplet.G_is_H);

            //there is supporting fact
            mind.AssertHolds(Triplet.X_is_Y);
        }

        [TestMethod]
        public void ComplexAndImplication_SameAsDefinition()
        {
            var mind = new MindModelTester();

            var same = "same";
            var Var1sameVar2 = TripletTree.Flat("$Var1", same, "$Var2");
            var Var1Var3Var4 = TripletTree.Flat("$Var1", "$Var3", "$Var4");
            var Var2Var3Var4 = TripletTree.Flat("$Var2", "$Var3", "$Var4");
            var isSameAndHasPropertyV4 = Triplet.And(Var1sameVar2, Var2Var3Var4);

            var sameAsDefinition = Triplet.Implication(isSameAndHasPropertyV4, Var1Var3Var4);
            var A_same_C = TripletTree.Flat("A", same, "C");
            mind.AddAxiom(sameAsDefinition);
            mind.AddAxiom(A_same_C);

            mind.AddAxiom(Triplet.C_is_B); 
            mind.AddAxiom(Triplet.C_is_D);

            //A should have same properties as C
            mind.AssertHolds(Triplet.A_is_B);
            mind.AssertHolds(Triplet.A_is_D);

            //there is no evidence for A nor C to has B
            //mind.AssertNotHolds(TripletTree.Flat("A", "has", "B"));
        }
    }
}
