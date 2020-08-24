/*
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
   public class Bisection : Solver1D
   {
      protected override double solveImpl(ISolver1d f, double xAccuracy)
      {

         /* The implementation of the algorithm was inspired by
            Press, Teukolsky, Vetterling, and Flannery,
            "Numerical Recipes in C", 2nd edition, Cambridge
            University Press
         */

         double dx, xMid, fMid;

         // Orient the search so that f>0 lies at root_+dx
         if (fxMin_ < 0.0)
         {
            dx = xMax_ - xMin_;
            root_ = xMin_;
         }
         else
         {
            dx = xMin_ - xMax_;
            root_ = xMax_;
         }

         while (evaluationNumber_ <= maxEvaluations_)
         {
            dx /= 2.0;
            xMid = root_ + dx;
            fMid = f.value(xMid);
            evaluationNumber_++;
            if (fMid <= 0.0)
               root_ = xMid;
            if (Math.Abs(dx) < xAccuracy || Utils.close(fMid, 0.0))
            {
               return root_;
            }
         }
         Utils.QL_FAIL("maximum number of function evaluations (" + maxEvaluations_ + ") exceeded",
                       QLNetExceptionEnum.MaxNumberFuncEvalExceeded);
         return 0;
      }
   }
}
