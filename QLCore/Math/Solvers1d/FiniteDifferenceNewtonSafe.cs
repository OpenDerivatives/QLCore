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
   public class FiniteDifferenceNewtonSafe : Solver1D
   {
      protected override double solveImpl(ISolver1d f, double xAccuracy)
      {
         // Orient the search so that f(xl) < 0
         double xh, xl;
         if (fxMin_ < 0.0)
         {
            xl = xMin_;
            xh = xMax_;
         }
         else
         {
            xh = xMin_;
            xl = xMax_;
         }

         double froot = f.value(root_);
         ++evaluationNumber_;
         // first order finite difference derivative
         double dfroot = xMax_ - root_ < root_ - xMin_ ?
                         (fxMax_ - froot) / (xMax_ - root_) :
                         (fxMin_ - froot) / (xMin_ - root_) ;

         // xMax_-xMin_>0 is verified in the constructor
         double dx = xMax_ - xMin_;
         while (evaluationNumber_ <= maxEvaluations_)
         {
            double frootold = froot;
            double rootold = root_;
            double dxold = dx;
            // Bisect if (out of range || not decreasing fast enough)
            if ((((root_ - xh)*dfroot - froot)*
                 ((root_ - xl)*dfroot - froot) > 0.0)
                || (Math.Abs(2.0 * froot) > Math.Abs(dxold * dfroot)))
            {

               dx = (xh - xl) / 2.0;
               root_ = xl + dx;
            }
            else     // Newton
            {
               dx = froot / dfroot;
               root_ -= dx;
            }

            // Convergence criterion
            if (Math.Abs(dx) < xAccuracy)
               return root_;

            froot = f.value(root_);
            ++evaluationNumber_;
            dfroot = (frootold - froot) / (rootold - root_);

            if (froot < 0.0)
               xl = root_;
            else
               xh = root_;
         }

         Utils.QL_FAIL("maximum number of function evaluations (" + maxEvaluations_ + ") exceeded",
                       QLNetExceptionEnum.MaxNumberFuncEvalExceeded);
         return 0;

      }
   }
}
