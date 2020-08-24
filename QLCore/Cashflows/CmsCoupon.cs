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
   //! CMS coupon class
   //    ! \warning This class does not perform any date adjustment,
   //                 i.e., the start and end date passed upon construction
   //                 should be already rolled to a business day.
   //
   public class CmsCoupon : FloatingRateCoupon
   {
      // need by CashFlowVectors
      public CmsCoupon() { }

      public CmsCoupon(double nominal,
                       Date paymentDate,
                       Date startDate,
                       Date endDate,
                       int fixingDays,
                       SwapIndex swapIndex,
                       double gearing = 1.0,
                       double spread = 0.0,
                       Date refPeriodStart = null,
                       Date refPeriodEnd = null,
                       DayCounter dayCounter = null,
                       bool isInArrears = false)
         : base(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears)
      {
         swapIndex_ = swapIndex;
      }
      // Inspectors
      public SwapIndex swapIndex()
      {
         return swapIndex_;
      }

      private SwapIndex swapIndex_;

      // Factory - for Leg generators
      public override CashFlow factory(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays,
                                       InterestRateIndex index, double gearing, double spread,
                                       Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
      {
         return new CmsCoupon(nominal, paymentDate, startDate, endDate, fixingDays,
                              (SwapIndex)index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears);
      }

   }


   //! helper class building a sequence of capped/floored cms-rate coupons
   public class CmsLeg : FloatingLegBase
   {
      public CmsLeg(Schedule schedule, SwapIndex swapIndex)
      {
         schedule_ = schedule;
         index_ = swapIndex;
         paymentAdjustment_ = BusinessDayConvention.Following;
         inArrears_ = false;
         zeroPayments_ = false;
      }

      public override List<CashFlow> value()
      {
         return CashFlowVectors.FloatingLeg<SwapIndex, CmsCoupon, CappedFlooredCmsCoupon>(
                   notionals_, schedule_, index_ as SwapIndex, paymentDayCounter_, paymentAdjustment_, fixingDays_, gearings_, spreads_, caps_, floors_, inArrears_, zeroPayments_);
      }
   }
}
