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
   //! Inverse cumulative Poisson distribution function
   /*! \test the correctness of the returned value is tested by
             checking it against known good results.
   */
   public class InverseCumulativePoisson : IValue
   {
      private double lambda_;

      public InverseCumulativePoisson() : this(1)
      {}

      public InverseCumulativePoisson(double lambda)
      {
         lambda_ = lambda;
         Utils.QL_REQUIRE(lambda_ > 0.0, () => "lambda must be positive");
      }

      public double value(double x)
      {
         Utils.QL_REQUIRE(x >= 0.0 && x <= 1.0, () =>
                          "Inverse cumulative Poisson distribution is only defined on the interval [0,1]");

         if (x.IsEqual(1.0))
            return double.MaxValue;

         double sum = 0.0;
         uint index = 0;
         while (x > sum)
         {
            sum += calcSummand(index);
            index++;
         }

         return index - 1;
      }

      private double calcSummand(uint index)
      {
         return Math.Exp(-lambda_) * Math.Pow(lambda_, index) / Factorial.get(index);
      }
   }
}
