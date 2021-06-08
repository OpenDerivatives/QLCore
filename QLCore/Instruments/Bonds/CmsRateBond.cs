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
   public class CmsRateBond : Bond
   {
      public CmsRateBond(int settlementDays,
                         double faceAmount,
                         Schedule schedule,
                         SwapIndex index,
                         DayCounter paymentDayCounter,
                         BusinessDayConvention paymentConvention = BusinessDayConvention.Following,
                         int fixingDays = 0,
                         List<double> gearings = null,
                         List<double> spreads = null,
                         List < double? > caps = null,
                         List < double? > floors = null,
                         bool inArrears = false,
                         double redemption = 100.0,
                         Date issueDate = null)
      : base(settlementDays, schedule.calendar(), issueDate)
      {
         // Optional value check
         if (gearings == null)
            gearings = new List<double>() {1};
         if (spreads == null)
            spreads = new List<double>() {0};
         if (caps == null)
            caps = new List < double? >();
         if (floors == null)
            floors = new List < double? >();

         maturityDate_ = schedule.endDate();
         cashflows_ = new CmsLeg(schedule, index)
         .withPaymentDayCounter(paymentDayCounter)
         .withFixingDays(fixingDays)
         .withGearings(gearings)
         .withSpreads(spreads)
         .withCaps(caps)
         .withFloors(floors)
         .inArrears(inArrears)
         .withNotionals(faceAmount)
         .withPaymentAdjustment(paymentConvention);

         addRedemptionsToCashflows(new List<double>() { redemption });

         Utils.QL_REQUIRE(cashflows().Count != 0, () => "bond with no cashflows!");
         Utils.QL_REQUIRE(redemptions_.Count == 1, () => "multiple redemptions created");
      }
   }
}