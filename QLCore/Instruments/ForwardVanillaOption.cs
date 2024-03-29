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

namespace QLCore
{
   public class ForwardVanillaOption : OneAssetOption
   {
      public ForwardVanillaOption(Settings settings,
                                  double moneyness,
                                  Date resetDate,
                                  StrikedTypePayoff payoff,
                                  Exercise exercise)
         : base(settings, payoff, exercise)
      {
         moneyness_ = moneyness;
         resetDate_ = resetDate;
      }

      public override void setupArguments(IPricingEngineArguments args)
      {
         base.setupArguments(args);
         ForwardVanillaOption.Arguments arguments = args as ForwardVanillaOption.Arguments;
         Utils.QL_REQUIRE(arguments != null, () => "wrong argument type");

         arguments.moneyness = moneyness_;
         arguments.resetDate = resetDate_;
         arguments.settings = settings_;
      }

      public override void fetchResults(IPricingEngineResults r)
      {
         base.fetchResults(r);
         ForwardVanillaOption.Results results = r as ForwardVanillaOption.Results;
         Utils.QL_REQUIRE(results != null, () => "no results returned from pricing engine");
         delta_       = results.delta;
         gamma_       = results.gamma;
         theta_       = results.theta;
         vega_        = results.vega;
         rho_         = results.rho;
         dividendRho_ = results.dividendRho;
      }

      // arguments
      private double moneyness_;
      private Date resetDate_;

      public new class Arguments : OneAssetOption.Arguments
      {
         public override void validate()
         {
            Utils.QL_REQUIRE(moneyness > 0.0, () => "negative or zero moneyness given");
            Utils.QL_REQUIRE(resetDate != null, () => "null reset date given");
            Utils.QL_REQUIRE(resetDate >= settings.evaluationDate(), () => "reset date in the past");
            Utils.QL_REQUIRE(this.exercise.lastDate() > resetDate, () => "reset date later or equal to maturity");
         }
         public double moneyness { get; set; }
         public Date resetDate { get; set; }
      }
   }
}
