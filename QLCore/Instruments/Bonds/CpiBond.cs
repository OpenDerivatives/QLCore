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
   //! cpi bond; if there is only one date in the schedule it
   //! is a zero bond returning an inflated notional.
   /*! \ingroup instruments

    */
   public class CPIBond : Bond
   {
      public CPIBond(int settlementDays,
                     double faceAmount,
                     bool growthOnly,
                     double baseCPI,
                     Period observationLag,
                     ZeroInflationIndex cpiIndex,
                     InterpolationType observationInterpolation,
                     Schedule schedule,
                     List<double> fixedRate,
                     DayCounter accrualDayCounter,
                     BusinessDayConvention paymentConvention = BusinessDayConvention.ModifiedFollowing,
                     Date issueDate = null,
                     Calendar paymentCalendar = null,
                     Period exCouponPeriod = null,
                     Calendar exCouponCalendar = null,
                     BusinessDayConvention exCouponConvention = BusinessDayConvention.Unadjusted,
                     bool exCouponEndOfMonth = false)
         : base(schedule.settings(), settlementDays, paymentCalendar ?? schedule.calendar(), issueDate)
      {
         frequency_ = schedule.tenor().frequency();
         dayCounter_ = accrualDayCounter;
         growthOnly_ = growthOnly;
         baseCPI_ = baseCPI;
         observationLag_ = observationLag;
         cpiIndex_ = cpiIndex;
         observationInterpolation_ = observationInterpolation;

         maturityDate_ = schedule.endDate();

         // a CPIleg know about zero legs and inclusion of base inflation notional
         cashflows_ = new CPILeg(schedule, cpiIndex_,
                                 baseCPI_, observationLag_)
         .withSubtractInflationNominal(growthOnly_)
         .withObservationInterpolation(observationInterpolation_)
         .withPaymentDayCounter(accrualDayCounter)
         .withFixedRates(fixedRate)
         .withPaymentCalendar(calendar_)
         .withExCouponPeriod(exCouponPeriod,
                             exCouponCalendar,
                             exCouponConvention,
                             exCouponEndOfMonth)
         .withNotionals(faceAmount)
         .withPaymentAdjustment(paymentConvention);
         
         calculateNotionalsFromCashflows();
      }

      public Frequency frequency() { return frequency_; }
      public DayCounter dayCounter() { return dayCounter_; }
      public bool growthOnly()  { return growthOnly_; }
      public double baseCPI()  { return baseCPI_; }
      public Period observationLag()  { return observationLag_; }
      public  ZeroInflationIndex cpiIndex()  { return cpiIndex_; }
      public InterpolationType observationInterpolation()  { return observationInterpolation_; }

      protected Frequency frequency_;
      protected DayCounter dayCounter_;
      protected bool growthOnly_;
      protected double baseCPI_;
      protected Period observationLag_;
      protected ZeroInflationIndex cpiIndex_;
      protected InterpolationType observationInterpolation_;
   }
}
