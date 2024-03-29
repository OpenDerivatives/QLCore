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
using System.Collections.Generic;

namespace QLCore
{
   //! Pricing engine for performance options using analytical formulae
   /*! \ingroup cliquetengines

       \test the correctness of the returned greeks is tested by
             reproducing numerical derivatives.
   */
   public class AnalyticPerformanceEngine : CliquetOption.Engine
   {
      public AnalyticPerformanceEngine(GeneralizedBlackScholesProcess process)
      {
         process_ = process;
      }
      public override void calculate()
      {
         Utils.QL_REQUIRE(arguments_.accruedCoupon == null &&
                          arguments_.lastFixing == null, () =>
                          "this engine cannot price options already started");
         Utils.QL_REQUIRE(arguments_.localCap == null &&
                          arguments_.localFloor == null &&
                          arguments_.globalCap == null &&
                          arguments_.globalFloor == null, () =>
                          "this engine cannot price capped/floored options");

         Utils.QL_REQUIRE(arguments_.exercise.type() == Exercise.Type.European, () => "not an European option");

         PercentageStrikePayoff moneyness = arguments_.payoff as PercentageStrikePayoff;
         Utils.QL_REQUIRE(moneyness != null, () => "wrong payoff given");

         List<Date> resetDates = arguments_.resetDates;
         resetDates.Add(arguments_.exercise.lastDate());

         double underlying = process_.stateVariable().link.value();
         Utils.QL_REQUIRE(underlying > 0.0, () => "negative or null underlying");

         StrikedTypePayoff payoff = new PlainVanillaPayoff(moneyness.optionType(), 1.0);

         results_.value = 0.0;
         results_.delta = results_.gamma = 0.0;
         results_.theta = 0.0;
         results_.rho = results_.dividendRho = 0.0;
         results_.vega = 0.0;

         for (int i = 1; i < resetDates.Count; i++)
         {
            double discount = process_.riskFreeRate().link.discount(resetDates[i - 1]);
            double rDiscount = process_.riskFreeRate().link.discount(resetDates[i]) /
                               process_.riskFreeRate().link.discount(resetDates[i - 1]);
            double qDiscount = process_.dividendYield().link.discount(resetDates[i]) /
                               process_.dividendYield().link.discount(resetDates[i - 1]);
            double forward = (1.0 / moneyness.strike()) * qDiscount / rDiscount;
            double variance = process_.blackVolatility().link.blackForwardVariance(
                                 resetDates[i - 1], resetDates[i],
                                 underlying * moneyness.strike());

            BlackCalculator black = new BlackCalculator(payoff, forward, Math.Sqrt(variance), rDiscount);

            DayCounter rfdc  = process_.riskFreeRate().link.dayCounter();
            DayCounter divdc = process_.dividendYield().link.dayCounter();
            DayCounter voldc = process_.blackVolatility().link.dayCounter();

            results_.value += discount * moneyness.strike() * black.value();
            results_.delta += 0.0;
            results_.gamma += 0.0;
            results_.theta += process_.riskFreeRate().link.forwardRate(
                                 resetDates[i - 1], resetDates[i], rfdc, Compounding.Continuous, Frequency.NoFrequency).value() *
                              discount * moneyness.strike() * black.value();

            double dt = rfdc.yearFraction(resetDates[i - 1], resetDates[i]);
            double t = rfdc.yearFraction(process_.riskFreeRate().link.referenceDate(), resetDates[i - 1]);
            results_.rho += discount * moneyness.strike() * (black.rho(dt) - t * black.value());

            dt = divdc.yearFraction(resetDates[i - 1], resetDates[i]);
            results_.dividendRho += discount * moneyness.strike() * black.dividendRho(dt);

            dt = voldc.yearFraction(resetDates[i - 1], resetDates[i]);
            results_.vega += discount * moneyness.strike() * black.vega(dt);
         }

      }

      public override void update()
      {
         process_.update();
         base.update();
      }
      private GeneralizedBlackScholesProcess process_;
   }
}
