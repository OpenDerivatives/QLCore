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
   public class GammaDistribution
   {
      private double a_;

      public GammaDistribution(double a)
      {
         a_ = a;
         Utils.QL_REQUIRE(a > 0.0, () => "invalid parameter for gamma distribution");
      }

      public double value(double x)
      {
         if (x <= 0.0)
            return 0.0;

         double gln = GammaFunction.logValue(a_);

         if (x < (a_ + 1.0))
         {
            double ap = a_;
            double del = 1.0 / a_;
            double sum = del;
            for (int n = 1; n <= 100; n++)
            {
               ap += 1.0;
               del *= x / ap;
               sum += del;
               if (Math.Abs(del) < Math.Abs(sum) * 3.0e-7)
                  return sum * Math.Exp(-x + a_ * Math.Log(x) - gln);
            }
         }
         else
         {
            double b = x + 1.0 - a_;
            double c = double.MaxValue;
            double d = 1.0 / b;
            double h = d;
            for (int n = 1; n <= 100; n++)
            {
               double an = -1.0 * n * (n - a_);
               b += 2.0;
               d = an * d + b;
               if (Math.Abs(d) < Const.QL_EPSILON)
                  d = Const.QL_EPSILON;
               c = b + an / c;
               if (Math.Abs(c) < Const.QL_EPSILON)
                  c = Const.QL_EPSILON;
               d = 1.0 / d;
               double del = d * c;
               h *= del;
               if (Math.Abs(del - 1.0) < Const.QL_EPSILON)
                  return h * Math.Exp(-x + a_ * Math.Log(x) - gln);
            }
         }
         Utils.QL_FAIL("too few iterations");
         return 0;
      }
   }

   //! Gamma function class
   /*! This is a function defined by
       \f[
           \Gamma(z) = \int_0^{\infty}t^{z-1}e^{-t}dt
       \f]

       The implementation of the algorithm was inspired by
       "Numerical Recipes in C", 2nd edition,
       Press, Teukolsky, Vetterling, Flannery, chapter 6

       \test the correctness of the returned value is tested by
             checking it against known good results.
   */

   public static class GammaFunction
   {
      const double c1_ = 76.18009172947146;
      const double c2_ = -86.50532032941677;
      const double c3_ = 24.01409824083091;
      const double c4_ = -1.231739572450155;
      const double c5_ = 0.1208650973866179e-2;
      const double c6_ = -0.5395239384953e-5;

      public static double logValue(double x)
      {
         Utils.QL_REQUIRE(x > 0.0, () => "positive argument required");

         double temp = x + 5.5;
         temp -= (x + 0.5) * Math.Log(temp);
         double ser = 1.000000000190015;
         ser += c1_ / (x + 1.0);
         ser += c2_ / (x + 2.0);
         ser += c3_ / (x + 3.0);
         ser += c4_ / (x + 4.0);
         ser += c5_ / (x + 5.0);
         ser += c6_ / (x + 6.0);

         return -temp + Math.Log(2.5066282746310005 * ser / x);
      }

      public static double value(double x)
      {
         if (x >= 1.0)
         {
            return Math.Exp(logValue(x));
         }
         if (x > -20.0)
         {
            return value(x + 1.0) / x;
         }
         return -Const.M_PI / (value(-x) * x * Math.Sin(Const.M_PI * x));
      }
   }

   public class CumulativeGammaDistribution
   {
      public CumulativeGammaDistribution(double a)
      {
         a_ = a;
         Utils.QL_REQUIRE(a > 0.0, () => "invalid parameter for gamma distribution");
      }

      public double value(double x)
      {
         if (x <= 0.0)
            return 0.0;

         double gln = GammaFunction.logValue(a_);

         if (x < (a_ + 1.0))
         {
            double ap = a_;
            double del = 1.0 / a_;
            double sum = del;
            for (int n = 1; n <= 100; n++)
            {
               ap += 1.0;
               del *= x / ap;
               sum += del;
               if (Math.Abs(del) < Math.Abs(sum) * 3.0e-7)
                  return sum * Math.Exp(-x + a_ * Math.Log(x) - gln);
            }
         }
         else
         {
            double b = x + 1.0 - a_;
            double c = Double.MaxValue;
            double d = 1.0 / b;
            double h = d;
            for (int n = 1; n <= 100; n++)
            {
               double an = -1.0 * n * (n - a_);
               b += 2.0;
               d = an * d + b;
               if (Math.Abs(d) < Const.QL_EPSILON)
                  d = Const.QL_EPSILON;
               c = b + an / c;
               if (Math.Abs(c) < Const.QL_EPSILON)
                  c = Const.QL_EPSILON;
               d = 1.0 / d;
               double del = d * c;
               h *= del;
               if (Math.Abs(del - 1.0) < Const.QL_EPSILON)
                  return 1.0 - h * Math.Exp(-x + a_ * Math.Log(x) - gln);
            }
         }
         Utils.QL_FAIL("too few iterations");
         return 0.0;
      }

      protected double a_;
   }
}
