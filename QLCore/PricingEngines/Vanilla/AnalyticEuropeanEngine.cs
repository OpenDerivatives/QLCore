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
   //! Pricing engine for European vanilla options using analytical formulae
   /*! \ingroup vanillaengines

       \test
       - the correctness of the returned value is tested by
         reproducing results available in literature.
       - the correctness of the returned greeks is tested by
         reproducing results available in literature.
       - the correctness of the returned greeks is tested by
         reproducing numerical derivatives.
       - the correctness of the returned implied volatility is tested
         by using it for reproducing the target value.
       - the implied-volatility calculation is tested by checking
         that it does not modify the option.
       - the correctness of the returned value in case of
         cash-or-nothing digital payoff is tested by reproducing
         results available in literature.
       - the correctness of the returned value in case of
         asset-or-nothing digital payoff is tested by reproducing
         results available in literature.
       - the correctness of the returned value in case of gap digital
         payoff is tested by reproducing results available in
         literature.
       - the correctness of the returned greeks in case of
         cash-or-nothing digital payoff is tested by reproducing
         numerical derivatives.
   */
   public class AnalyticEuropeanEngine : VanillaOption.Engine
   {
      private GeneralizedBlackScholesProcess process_;

      public AnalyticEuropeanEngine(GeneralizedBlackScholesProcess process)
      {
         process_ = process;
      }

      public override void update()
      {
        process_.update();
        base.update();
      }

      public override void calculate()
      {

         Utils.QL_REQUIRE(arguments_.exercise.type() == Exercise.Type.European, () => "not an European option");

         StrikedTypePayoff payoff = arguments_.payoff as StrikedTypePayoff;
         Utils.QL_REQUIRE(payoff != null, () => "non-striked payoff given");

         double variance = process_.blackVolatility().link.blackVariance(arguments_.exercise.lastDate(), payoff.strike());
         double dividendDiscount = process_.dividendYield().link.discount(arguments_.exercise.lastDate());
         double riskFreeDiscount = process_.riskFreeRate().link.discount(arguments_.exercise.lastDate());
         double spot = process_.stateVariable().link.value();
         Utils.QL_REQUIRE(spot > 0.0, () => "negative or null underlying given");
         double forwardPrice = spot * dividendDiscount / riskFreeDiscount;

         BlackCalculator black = new BlackCalculator(payoff, forwardPrice, Math.Sqrt(variance), riskFreeDiscount);

         results_.value = black.value();
         results_.delta = black.delta(spot);
         results_.deltaForward = black.deltaForward();
         results_.elasticity = black.elasticity(spot);
         results_.gamma = black.gamma(spot);

         DayCounter rfdc  = process_.riskFreeRate().link.dayCounter();
         DayCounter divdc = process_.dividendYield().link.dayCounter();
         DayCounter voldc = process_.blackVolatility().link.dayCounter();
         double t = rfdc.yearFraction(process_.riskFreeRate().link.referenceDate(), arguments_.exercise.lastDate());
         results_.rho = black.rho(t);

         t = divdc.yearFraction(process_.dividendYield().link.referenceDate(), arguments_.exercise.lastDate());
         results_.dividendRho = black.dividendRho(t);

         t = voldc.yearFraction(process_.blackVolatility().link.referenceDate(), arguments_.exercise.lastDate());
         results_.vega = black.vega(t);
         try
         {
            results_.theta = black.theta(spot, t);
            results_.thetaPerDay = black.thetaPerDay(spot, t);
         }
         catch
         {
            results_.theta = null;
            results_.thetaPerDay = null;
         }

         results_.strikeSensitivity  = black.strikeSensitivity();
         results_.itmCashProbability = black.itmCashProbability();
      }
   }
}
