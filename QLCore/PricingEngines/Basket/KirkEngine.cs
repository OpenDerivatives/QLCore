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
   //! Pricing engine for spread option on two futures
   /*! This class implements formulae from
       "Correlation in the Energy Markets", E. Kirk
       Managing Energy Price Risk.
       London: Risk Publications and Enron, pp. 71-78

       \ingroup basketengines

       \test the correctness of the returned value is tested by
             reproducing results available in literature.
   */
   public class KirkEngine : BasketOption.Engine
   {
      public KirkEngine(BlackProcess process1,
                        BlackProcess process2,
                        double correlation)
      {
         process1_ = process1;
         process2_ = process2;
         rho_ = correlation;
      }

      public override void calculate()
      {

         Utils.QL_REQUIRE(arguments_.exercise.type() == Exercise.Type.European, () => "not an European Option");

         EuropeanExercise exercise = arguments_.exercise as EuropeanExercise;
         Utils.QL_REQUIRE(exercise != null, () => "not an European Option");

         SpreadBasketPayoff spreadPayoff = arguments_.payoff as SpreadBasketPayoff;
         Utils.QL_REQUIRE(spreadPayoff != null, () => " spread payoff expected");

         PlainVanillaPayoff payoff = spreadPayoff.basePayoff() as PlainVanillaPayoff;
         Utils.QL_REQUIRE(payoff != null, () => "non-plain payoff given");
         double strike = payoff.strike();

         double f1 = process1_.stateVariable().link.value();
         double f2 = process2_.stateVariable().link.value();

         // use atm vols
         double variance1 = process1_.blackVolatility().link.blackVariance(exercise.lastDate(), f1);
         double variance2 = process2_.blackVolatility().link.blackVariance(exercise.lastDate(), f2);

         double riskFreeDiscount = process1_.riskFreeRate().link.discount(exercise.lastDate());

         Func<double, double> Square = x => x * x;
         double f = f1 / (f2 + strike);
         double v = Math.Sqrt(variance1
                              + variance2 * Square(f2 / (f2 + strike))
                              - 2 * rho_ * Math.Sqrt(variance1 * variance2)
                              * (f2 / (f2 + strike)));

         BlackCalculator black = new BlackCalculator(new PlainVanillaPayoff(payoff.optionType(), 1.0), f, v, riskFreeDiscount);

         results_.value = (f2 + strike) * black.value();

      }

      private BlackProcess process1_;
      private BlackProcess process2_;
      private  double rho_;
   }
}
