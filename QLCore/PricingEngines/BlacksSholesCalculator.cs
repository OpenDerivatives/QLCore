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

   //! Black-Scholes 1973 calculator class
   public class BlackScholesCalculator : BlackCalculator
   {
      protected double spot_;
      protected double growth_;

      public BlackScholesCalculator(StrikedTypePayoff payoff, double spot, double growth, double stdDev, double discount)
         : base(payoff, spot* growth / discount, stdDev, discount)
      {
         spot_ = spot;
         growth_ = growth;

         Utils.QL_REQUIRE(spot_ >= 0.0, () => "positive spot value required: " + spot_ + " not allowed");
         Utils.QL_REQUIRE(growth_ >= 0.0, () => "positive growth value required: " + growth_ + " not allowed");
      }

      //! Sensitivity to change in the underlying spot price.

      public double delta()
      {
         return base.delta(spot_);
      }

//        ! Sensitivity in percent to a percent change in the
//            underlying spot price.
      public double elasticity()
      {
         return base.elasticity(spot_);
      }

//        ! Second order derivative with respect to change in the
//            underlying spot price.
      public double gamma()
      {
         return base.gamma(spot_);
      }
      //! Sensitivity to time to maturity.
      public double theta(double maturity)
      {
         return base.theta(spot_, maturity);
      }
//        ! Sensitivity to time to maturity per day
//            (assuming 365 day in a year).
      public double thetaPerDay(double maturity)
      {
         return base.thetaPerDay(spot_, maturity);
      }
   }
}
