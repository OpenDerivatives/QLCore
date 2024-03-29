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

using System.Collections.Generic;

namespace QLCore
{
   //! Single-asset barrier option with discrete dividends
   /*! \ingroup instruments */
   public class DividendBarrierOption : BarrierOption
   {
      public DividendBarrierOption(Settings settings,
                                   Barrier.Type barrierType,
                                   double barrier,
                                   double rebate,
                                   StrikedTypePayoff payoff,
                                   Exercise exercise,
                                   List<Date> dividendDates,
                                   List<double> dividends)
         : base(settings, barrierType, barrier, rebate, payoff, exercise)
      {
         cashFlow_ = Utils.DividendVector(settings, dividendDates, dividends);
      }

      public override void setupArguments(IPricingEngineArguments args)
      {
         base.setupArguments(args);

         DividendBarrierOption.Arguments arguments = args as DividendBarrierOption.Arguments;
         Utils.QL_REQUIRE(arguments != null, () => "wrong engine type");

         arguments.cashFlow = cashFlow_;
      }

      private DividendSchedule cashFlow_;


      //! %Arguments for dividend barrier option calculation
      public new class Arguments : BarrierOption.Arguments
      {
         public DividendSchedule cashFlow { get; set; }
         public Arguments()
          : base()
         {
            cashFlow = new DividendSchedule();
         }
         public override void validate()
         {
            base.validate();

            Date exerciseDate = exercise.lastDate();

            for (int i = 0; i < cashFlow.Count; i++)
            {
               Utils.QL_REQUIRE(cashFlow[i].date() <= exerciseDate, () =>
                                "the " + (i + 1) + " dividend date (" + cashFlow[i].date() + ") is later than the exercise date ("
                                + exerciseDate + ")");
            }
         }
      }
      //! %Dividend-barrier-option %engine base class
      public new class Engine :  GenericEngine<DividendBarrierOption.Arguments, DividendBarrierOption.Results> {}

   }

}

