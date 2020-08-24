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
   //! %Coupon paying a YoY-inflation type index
   public class YoYInflationCoupon : InflationCoupon
   {
      public YoYInflationCoupon(Date paymentDate,
                                double nominal,
                                Date startDate,
                                Date endDate,
                                int fixingDays,
                                YoYInflationIndex yoyIndex,
                                Period observationLag,
                                DayCounter dayCounter,
                                double gearing = 1.0,
                                double spread = 0.0,
                                Date refPeriodStart = null,
                                Date refPeriodEnd = null)
         : base(paymentDate, nominal, startDate, endDate,
                fixingDays, yoyIndex, observationLag,
                dayCounter, refPeriodStart, refPeriodEnd)
      {
         yoyIndex_ = yoyIndex;
         gearing_ = gearing;
         spread_ = spread;
      }

      // Inspectors
      // index gearing, i.e. multiplicative coefficient for the index
      public double gearing() { return gearing_; }
      //! spread paid over the fixing of the underlying index
      public double spread() { return spread_; }
      public double adjustedFixing() { return (rate() - spread()) / gearing(); }
      public YoYInflationIndex yoyIndex() { return yoyIndex_; }

      private YoYInflationIndex yoyIndex_;
      protected double gearing_;
      protected double spread_;

      protected override bool checkPricerImpl(InflationCouponPricer i)
      {
         return (i is YoYInflationCouponPricer);
      }
   }


   //! Helper class building a sequence of capped/floored yoy inflation coupons
   //! payoff is: spread + gearing x index
   public class yoyInflationLeg : yoyInflationLegBase
   {
      public yoyInflationLeg(Schedule schedule, Calendar cal,
                             YoYInflationIndex index,
                             Period observationLag)
      {
         schedule_ = schedule;
         index_ = index;
         observationLag_ = observationLag;
         paymentAdjustment_ = BusinessDayConvention.ModifiedFollowing;
         paymentCalendar_ = cal;
      }


      public override List<CashFlow> value()
      {
         return CashFlowVectors.yoyInflationLeg(notionals_,
                                                schedule_,
                                                paymentAdjustment_,
                                                index_,
                                                gearings_,
                                                spreads_,
                                                paymentDayCounter_,
                                                caps_,
                                                floors_,
                                                paymentCalendar_,
                                                fixingDays_,
                                                observationLag_);
      }

   }
}
