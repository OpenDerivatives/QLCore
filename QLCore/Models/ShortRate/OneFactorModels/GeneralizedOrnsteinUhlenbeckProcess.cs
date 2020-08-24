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
   /// <summary>
   /// Piecewise linear Ornstein-Uhlenbeck process class
   /// <remarks>
   /// This class describes the Ornstein-Uhlenbeck process governed by
   ///      dx = a (level - x_t) dt + \sigma dW_t
   /// where the coefficients a and sigma are piecewise linear.
   /// </remarks>
   /// </summary>
   public class GeneralizedOrnsteinUhlenbeckProcess : StochasticProcess1D
   {
      public GeneralizedOrnsteinUhlenbeckProcess(Func<double, double> speed,
                                                 Func<double, double> vol,
                                                 double x0 = 0.0,
                                                 double level = 0.0)
      {
         x0_ = x0;
         speed_ = speed;
         level_ = level;
         volatility_ = vol;
         Utils.QL_REQUIRE(x0 >= 0.0, () => "negative initial data given");
         Utils.QL_REQUIRE(level >= 0.0, () => "negative level given");
      }
      // StochasticProcess interface
      public override double x0()
      {
         return x0_;
      }
      public double speed(double t)
      {
         return speed_(t);
      }
      public double volatility(double t)
      {
         return volatility_(t);
      }
      public double level()
      {
         return level_;
      }
      public override double drift(double t, double x)
      {
         return speed_(t) * (level_ - x);
      }
      public override double diffusion(double t, double UnnamedParameter2)
      {
         return volatility_(t);
      }
      public override double expectation(double t, double x0, double dt)
      {
         return level_ + (x0 - level_) * Math.Exp(-speed_(t) * dt);
      }
      public override double stdDeviation(double t, double x0, double dt)
      {
         return Math.Sqrt(variance(t, x0, dt));
      }
      public override double variance(double t, double UnnamedParameter2, double dt)
      {
         if (speed_(t) < Math.Sqrt(((Const.QL_EPSILON))))
         {
            // algebric limit for small speed
            return volatility_(t) * volatility_(t) * dt;
         }
         else
         {
            return 0.5 * volatility_(t) * volatility_(t) / speed_(t) * (1.0 - Math.Exp(-2.0 * speed_(t) * dt));
         }
      }

      private double x0_;
      private Func<double, double> speed_;
      private double level_;
      private Func<double, double> volatility_;

   }

}
