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

   //! Square-root process class
//    ! This class describes a square-root process governed by
//        \f[
//            dx = a (b - x_t) dt + \sigma \sqrt{x_t} dW_t.
//        \f]
//
//        \ingroup processes
//
   public class SquareRootProcess : StochasticProcess1D
   {
      private double x0_;
      private double mean_;
      private double speed_;
      private double volatility_;

      public SquareRootProcess(double b, double a, double sigma, double x0) : this(b, a, sigma, x0, new EulerDiscretization())
      {
      }
      public SquareRootProcess(double b, double a, double sigma) : this(b, a, sigma, 0.0, new EulerDiscretization())
      {
      }
      public SquareRootProcess(double b, double a, double sigma, double x0, IDiscretization1D disc)
         : base(disc)
      {
         x0_ = x0;
         mean_ = b;
         speed_ = a;
         volatility_ = sigma;
      }
      // StochasticProcess interface
      public override double x0()
      {
         return x0_;
      }
      public override double drift(double UnnamedParameter1, double x)
      {
         return speed_ * (mean_ - x);
      }
      public override double diffusion(double UnnamedParameter1, double x)
      {
         return volatility_ * Math.Sqrt(x);
      }
   }

}
