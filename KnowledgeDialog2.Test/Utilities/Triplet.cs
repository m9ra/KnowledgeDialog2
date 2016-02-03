using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnowledgeDialog2.Database;

namespace KnowledgeDialog2.Test.Utilities
{
    class Triplet
    {
        #region Precreated triplets

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree Var1_is_B = TripletTree.Flat("$Var1", "is", "B");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree A_is_B = TripletTree.Flat("A", "is", "B");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree C_is_B = TripletTree.Flat("C", "is", "B");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree C_is_D = TripletTree.Flat("C", "is", "D");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree E_is_F = TripletTree.Flat("E", "is", "F");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree G_is_H = TripletTree.Flat("G", "is", "H");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree A_is_D = TripletTree.Flat("A", "is", "D");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree X_is_Y = TripletTree.Flat("X", "is", "Y");

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree AB_implies_CD = Implication(A_is_B, C_is_D);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree CD_implies_EF = Implication(C_is_D, E_is_F);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree EF_implies_GH = Implication(E_is_F, G_is_H);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree AB_implies_AD = Implication(A_is_B, A_is_D);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree CD_implies_Var1B = Implication(C_is_D, Var1_is_B);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree AB_implies_CB = Implication(A_is_B, C_is_B);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree CD_implies_XY = Implication(C_is_D, X_is_Y);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree AB_and_CD_implies_EF = Implication(And(A_is_B, C_is_D), E_is_F);

        /// <summary>
        /// Utility triplet for testing.
        /// </summary>
        internal static readonly TripletTree EF_and_GH_implies_XY = Implication(And(E_is_F, G_is_H), X_is_Y);

        #endregion

        /// <summary>
        /// Creates implication between given triplets.
        /// </summary>
        /// <param name="triplet1">Precondition triplet.</param>
        /// <param name="triplet2">Dependent triplet.</param>
        /// <returns>The implication.</returns>
        internal static TripletTree Implication(TripletTree triplet1, TripletTree triplet2)
        {
            return TripletTree.From(triplet1, MindModel.Inference.ImplicationRule.ThenPredicate, triplet2);
        }

        /// <summary>
        /// Creates and between given triplets.
        /// </summary>
        /// <param name="triplet1">First triplet.</param>
        /// <param name="triplet2">Second triplet.</param>
        /// <returns>The conjunction.</returns>
        internal static TripletTree And(TripletTree triplet1, TripletTree triplet2)
        {
            return TripletTree.From(triplet1, MindModel.Inference.AndRule.AndPredicate, triplet2);
        }
    }
}
