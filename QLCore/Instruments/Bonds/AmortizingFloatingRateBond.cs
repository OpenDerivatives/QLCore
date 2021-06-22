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

using System.Collections.Generic;

namespace QLCore
{
   //! amortizing floating-rate bond (possibly capped and/or floored)
   public class AmortizingFloatingRateBond : Bond
   {
      public AmortizingFloatingRateBond(int settlementDays,
                                        List<double> notionals,
                                        Schedule schedule,
                                        IborIndex index,
                                        DayCounter accrualDayCounter,
                                        BusinessDayConvention paymentConvention = BusinessDayConvention.Following,
                                        int fixingDays = 0,
                                        List<double> gearings = null,
                                        List<double> spreads = null,
                                        List < double? > caps = null,
                                        List < double? > floors = null,
                                        bool inArrears = false,
                                        Date issueDate = null)
      : base(schedule.settings(), settlementDays, schedule.calendar(), issueDate)
      {
         if (gearings == null)
            gearings = new List<double>() {1, 1.0};

         if (spreads == null)
            spreads = new List<double>() { 1, 0.0 };

         if (caps == null)
            caps = new List < double? >() ;

         if (floors == null)
            floors = new List < double? >();

         maturityDate_ = schedule.endDate();


         cashflows_ = new IborLeg(schedule, index)
         .withCaps(caps)
         .withFloors(floors)
         .inArrears(inArrears)
         .withSpreads(spreads)
         .withGearings(gearings)
         .withFixingDays(fixingDays)
         .withPaymentDayCounter(accrualDayCounter)
         .withPaymentAdjustment(paymentConvention)
         .withNotionals(notionals).value();

         addRedemptionsToCashflows();

         Utils.QL_REQUIRE(!cashflows().empty(), () => "bond with no cashflows!");
      }
   }
}
