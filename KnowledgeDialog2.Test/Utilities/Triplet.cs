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
    }
}
