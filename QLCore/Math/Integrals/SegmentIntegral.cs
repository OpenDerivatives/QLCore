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

   //! Integral of a one-dimensional function
   //    ! Given a number \f$ N \f$ of intervals, the integral of
   //        a function \f$ f \f$ between \f$ a \f$ and \f$ b \f$ is
   //        calculated by means of the trapezoid formula
   //        \f[
   //        \int_{a}^{b} f \mathrm{d}x =
   //        \frac{1}{2} f(x_{0}) + f(x_{1}) + f(x_{2}) + \dots
   //        + f(x_{N-1}) + \frac{1}{2} f(x_{N})
   //        \f]
   //        where \f$ x_0 = a \f$, \f$ x_N = b \f$, and
   //        \f$ x_i = a+i \Delta x \f$ with
   //        \f$ \Delta x = (b-a)/N \f$.
   //
   //        \test the correctness of the result is tested by checking it
   //              against known good values.
   //
   public class SegmentIntegral : Integrator
   {
      private int intervals_;

      public SegmentIntegral(int intervals)
         : base(1, 1)
      {
         intervals_ = intervals;

         Utils.QL_REQUIRE(intervals > 0, () => "at least 1 interval needed, 0 given");
      }

      // inline and template definitions
      protected override double integrate(Func<double, double> f, double a, double b)
      {
         double dx = (b - a) / intervals_;
         double sum = 0.5 * (f(a) + f(b));
         double end = b - 0.5 * dx;
         for (double x = a + dx; x < end; x += dx)
            sum += f(x);
         return sum * dx;
      }
   }

}
