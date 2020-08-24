﻿/*
 Copyright (C) 2020 Jean-Camille Tournier (mail@tournierjc.fr)

 This file is part of QLCore Project https://github.com/OpenDerivatives/QLCore

 QLCore is free software: you can redistribute it and/or modify it
 under the terms of the QLCore and QLNet license. You should have received a
 copy of the license along with this program; if not, license is
 available at https://github.com/OpenDerivatives/QLCore/LICENSE.

 QLCore is a forked of QLNet which is a based on QuantLib, a free-software/open-source
 library for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml and the
 QLNet license is available online at https://github.com/amaggiulli/QLNet/blob/develop/LICENSE.

 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICAR PURPOSE. See the license for more details.
*/

using System;

namespace QLCore
{
   public static partial class MatrixUtilities
   {
      public static Matrix CholeskyDecomposition(Matrix S, bool flexible)
      {
         int i, j, size = S.rows();

         Utils.QL_REQUIRE(size == S.columns(), () => "input matrix is not a square matrix");
#if QL_EXTRA_SAFETY_CHECKS
         for (i = 0; i < S.rows(); i++)
            for (j = 0; j < i; j++)
               QL_REQUIRE(S[i][j] == S[j][i], () =>
                          "input matrix is not symmetric");
#endif

         Matrix result = new Matrix(size, size, 0.0);
         double sum;
         for (i = 0; i < size; i++)
         {
            for (j = i; j < size; j++)
            {
               sum = S[i, j];
               for (int k = 0; k <= i - 1; k++)
               {
                  sum -= result[i, k] * result[j, k];
               }
               if (i == j)
               {
                  Utils.QL_REQUIRE(flexible || sum > 0.0, () => "input matrix is not positive definite");
                  // To handle positive semi-definite matrices take the
                  // square root of sum if positive, else zero.
                  result[i, i] = Math.Sqrt(Math.Max(sum, 0.0));
               }
               else
               {
                  // With positive semi-definite matrices is possible
                  // to have result[i][i]==0.0
                  // In this case sum happens to be zero as well
                  result[j, i] = (sum.IsEqual(0.0) ? 0.0 : sum / result[i, i]);
               }
            }
         }
         return result;
      }
   }
}
