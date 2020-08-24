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
   public class Secant : Solver1D
   {
      protected override double solveImpl(ISolver1d f, double xAccuracy)
      {

         /* The implementation of the algorithm was inspired by
            Press, Teukolsky, Vetterling, and Flannery,
            "Numerical Recipes in C", 2nd edition, Cambridge
            University Press
         */

         double fl, froot, dx, xl;

         // Pick the bound with the smaller function value
         // as the most recent guess
         if (Math.Abs(fxMin_) < Math.Abs(fxMax_))
         {
            root_ = xMin_;
            froot = fxMin_;
            xl = xMax_;
            fl = fxMax_;
         }
         else
         {
            root_ = xMax_;
            froot = fxMax_;
            xl = xMin_;
            fl = fxMin_;
         }
         while (evaluationNumber_ <= maxEvaluations_)
         {
            dx = (xl - root_) * froot / (froot - fl);
            xl = root_;
            fl = froot;
            root_ += dx;
            froot = f.value(root_);
            ++evaluationNumber_;
            if (Math.Abs(dx) < xAccuracy || Utils.close(froot, 0.0))
               return root_;
         }

         Utils.QL_FAIL("maximum number of function evaluations (" + maxEvaluations_ + ") exceeded",
                       QLNetExceptionEnum.MaxNumberFuncEvalExceeded);
         return 0;
      }
   }
}
