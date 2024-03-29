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
   //! Vanilla option (no discrete dividends, no barriers) on a single asset
   public class VanillaOption : OneAssetOption
   {
      public VanillaOption(Settings settings, StrikedTypePayoff payoff, Exercise exercise)
         : base(settings, payoff, exercise) {}

      /*! \warning currently, this method returns the Black-Scholes
               implied volatility using analytic formulas for
               European options and a finite-difference method
               for American and Bermudan options. It will give
               unconsistent results if the pricing was performed
               with any other methods (such as jump-diffusion
               models.)

      \warning options with a gamma that changes sign (e.g.,
               binary options) have values that are <b>not</b>
               monotonic in the volatility. In these cases, the
               calculation can fail and the result (if any) is
               almost meaningless.  Another possible source of
               failure is to have a target value that is not
               attainable with any volatility, e.g., a target
               value lower than the intrinsic value in the case
               of American options.
      */
      public double impliedVolatility(double targetValue,
                                      GeneralizedBlackScholesProcess process,
                                      double accuracy = 1.0e-4,
                                      int maxEvaluations = 100,
                                      double minVol = 1.0e-7,
                                      double maxVol = 4.0)
      {

         Utils.QL_REQUIRE(!isExpired(), () => "option expired");

         SimpleQuote volQuote = new SimpleQuote();

         GeneralizedBlackScholesProcess newProcess = ImpliedVolatilityHelper.clone(process, volQuote);

         // engines are built-in for the time being
         IPricingEngine engine;
         switch (exercise_.type())
         {
            case Exercise.Type.European:
               engine = new AnalyticEuropeanEngine(newProcess);
               break;
            case Exercise.Type.American:
               engine = new FDAmericanEngine(newProcess);
               break;
            case Exercise.Type.Bermudan:
               engine = new FDBermudanEngine(newProcess);
               break;
            default:
               throw new ArgumentException("unknown exercise type");
         }

         return ImpliedVolatilityHelper.calculate(this, engine, volQuote, targetValue, accuracy,
                                                  maxEvaluations, minVol, maxVol);
      }
   }
}
