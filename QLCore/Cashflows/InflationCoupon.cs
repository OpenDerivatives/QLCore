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
   //! Base inflation-coupon class
   /*! The day counter is usually obtained from the inflation term
       structure that the inflation index uses for forecasting.
       There is no gearing or spread because these are relevant for
       YoY coupons but not zero inflation coupons.

       \note inflation indices do not contain day counters or calendars.
   */
   public class InflationCoupon : Coupon
   {

      public InflationCoupon(Date paymentDate,
                             double nominal,
                             Date startDate,
                             Date endDate,
                             int fixingDays,
                             InflationIndex index,
                             Period observationLag,
                             DayCounter dayCounter,
                             Date refPeriodStart = null,
                             Date refPeriodEnd = null,
                             Date exCouponDate = null)
         : base(index.settings(), paymentDate, nominal, startDate, endDate, refPeriodStart, refPeriodEnd, exCouponDate)    // ref period is before lag
      {
         index_ = index;
         observationLag_ = observationLag;
         dayCounter_ = dayCounter;
         fixingDays_ = fixingDays;
      }

      // CashFlow interface
      public override double amount()
      {
         return rate() * accrualPeriod() * nominal();
      }
      // Coupon interface
      public double price(Handle<YieldTermStructure> discountingCurve)
      {
         return amount() * discountingCurve.link.discount(date());
      }
      public override DayCounter dayCounter() { return dayCounter_; }
      public override double accruedAmount(Date d)
      {
         if (d <= accrualStartDate_ || d > paymentDate_)
         {
            return 0.0;
         }
         else
         {
            return nominal() * rate() *
                   dayCounter().yearFraction(accrualStartDate_,
                                             d < accrualEndDate_ ? d : accrualEndDate_, //Math.Min(d, accrualEndDate_),
                                             refPeriodStart_,
                                             refPeriodEnd_);
         }
      }
      public override double rate()
      {
         Utils.QL_REQUIRE(pricer_ != null, () => "pricer not set");

         // we know it is the correct type because checkPricerImpl checks on setting
         // in general pricer_ will be a derived class, as will *this on calling
         pricer_.initialize(this);
         return pricer_.swapletRate();
      }

      // Inspectors
      //! yoy inflation index
      public InflationIndex index() { return index_; }
      //! how the coupon observes the index
      public Period observationLag() { return observationLag_; }
      //! fixing days
      public int fixingDays() { return fixingDays_; }
      //! fixing date
      public virtual Date fixingDate()
      {
         // fixing calendar is usually the null calendar for inflation indices
         return index_.fixingCalendar().advance(refPeriodEnd_ - observationLag_,
                                                -(fixingDays_), TimeUnit.Days, BusinessDayConvention.ModifiedPreceding);
      }
      //! fixing of the underlying index, as observed by the coupon
      public virtual double indexFixing()
      {
         return index_.fixing(fixingDate());
      }

      public void setPricer(InflationCouponPricer pricer)
      {
         Utils.QL_REQUIRE(checkPricerImpl(pricer), () => "pricer given is wrong type");
         pricer_ = pricer;
      }

      public InflationCouponPricer pricer() {return pricer_;}

      protected InflationCouponPricer pricer_;
      protected InflationIndex index_;
      protected Period observationLag_;
      protected DayCounter dayCounter_;
      protected int fixingDays_;

      //! makes sure you were given the correct type of pricer
      // this can also done in external pricer setter classes via
      // accept/visit mechanism
      protected virtual bool checkPricerImpl(InflationCouponPricer i) { return false; }

   }
}
