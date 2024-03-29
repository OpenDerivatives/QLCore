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

using Xunit;
using QLCore;
using System;
using System.Collections.Generic;

namespace TestSuite
{
   public class T_CashFlows
   {
      private void CHECK_INCLUSION(int n, int days, bool expected, List<CashFlow> leg, Date today)
      {
         if ((!leg[n].hasOccurred(today + days)) != expected)
         {
            QAssert.Fail("cashflow at T+" + n + " "
                         + (expected ? "not" : "") + "included"
                         + " at T+" + days);
         }
      }

      private void CHECK_NPV(bool includeRef, double expected, InterestRate no_discount, List<CashFlow> leg, Date today)
      {
         do
         {
            double NPV = CashFlows.npv(leg, no_discount, includeRef, today);
            if (Math.Abs(NPV - expected) > 1e-6)
            {
               QAssert.Fail("NPV mismatch:\n"
                            + "    calculated: " + NPV + "\n"
                            + "    expected: " + expected);
            }
         }
         while (false);
      }

      [Fact]
      public void testSettings()
      {
         // Testing cash-flow settings...
         using (Settings settings = new Settings())
         {
            Date today = Date.Today;
            settings.setEvaluationDate(today);

            // cash flows at T+0, T+1, T+2
            List<CashFlow> leg = new List<CashFlow>();

            for (int i = 0; i < 3; ++i)
               leg.Add(new SimpleCashFlow(settings, 1.0, today + i));

            // case 1: don't include reference-date payments, no override at
            //         today's date

            settings.includeReferenceDateEvents = false;
            settings.includeTodaysCashFlows = null;

            CHECK_INCLUSION(0, 0, false, leg, today);
            CHECK_INCLUSION(0, 1, false, leg, today);

            CHECK_INCLUSION(1, 0, true, leg, today);
            CHECK_INCLUSION(1, 1, false, leg, today);
            CHECK_INCLUSION(1, 2, false, leg, today);

            CHECK_INCLUSION(2, 1, true, leg, today);
            CHECK_INCLUSION(2, 2, false, leg, today);
            CHECK_INCLUSION(2, 3, false, leg, today);

            // case 2: same, but with explicit setting at today's date

            settings.includeReferenceDateEvents = false;
            settings.includeTodaysCashFlows = false;

            CHECK_INCLUSION(0, 0, false, leg, today);
            CHECK_INCLUSION(0, 1, false, leg, today);

            CHECK_INCLUSION(1, 0, true, leg, today);
            CHECK_INCLUSION(1, 1, false, leg, today);
            CHECK_INCLUSION(1, 2, false, leg, today);

            CHECK_INCLUSION(2, 1, true, leg, today);
            CHECK_INCLUSION(2, 2, false, leg, today);
            CHECK_INCLUSION(2, 3, false, leg, today);

            // case 3: do include reference-date payments, no override at
            //         today's date

            settings.includeReferenceDateEvents = true;
            settings.includeTodaysCashFlows = null;

            CHECK_INCLUSION(0, 0, true, leg, today);
            CHECK_INCLUSION(0, 1, false, leg, today);

            CHECK_INCLUSION(1, 0, true, leg, today);
            CHECK_INCLUSION(1, 1, true, leg, today);
            CHECK_INCLUSION(1, 2, false, leg, today);

            CHECK_INCLUSION(2, 1, true, leg, today);
            CHECK_INCLUSION(2, 2, true, leg, today);
            CHECK_INCLUSION(2, 3, false, leg, today);

            // case 4: do include reference-date payments, explicit (and same)
            //         setting at today's date

            settings.includeReferenceDateEvents = true;
            settings.includeTodaysCashFlows = true;

            CHECK_INCLUSION(0, 0, true, leg, today);
            CHECK_INCLUSION(0, 1, false, leg, today);

            CHECK_INCLUSION(1, 0, true, leg, today);
            CHECK_INCLUSION(1, 1, true, leg, today);
            CHECK_INCLUSION(1, 2, false, leg, today);

            CHECK_INCLUSION(2, 1, true, leg, today);
            CHECK_INCLUSION(2, 2, true, leg, today);
            CHECK_INCLUSION(2, 3, false, leg, today);

            // case 5: do include reference-date payments, override at
            //         today's date

            settings.includeReferenceDateEvents = true;
            settings.includeTodaysCashFlows = false;

            CHECK_INCLUSION(0, 0, false, leg, today);
            CHECK_INCLUSION(0, 1, false, leg, today);

            CHECK_INCLUSION(1, 0, true, leg, today);
            CHECK_INCLUSION(1, 1, true, leg, today);
            CHECK_INCLUSION(1, 2, false, leg, today);

            CHECK_INCLUSION(2, 1, true, leg, today);
            CHECK_INCLUSION(2, 2, true, leg, today);
            CHECK_INCLUSION(2, 3, false, leg, today);

            // no discount to make calculations easier
            InterestRate no_discount = new InterestRate(0.0, new Actual365Fixed(), Compounding.Continuous, Frequency.Annual);

            // no override
            settings.includeTodaysCashFlows = null;

            CHECK_NPV(false, 2.0, no_discount, leg, today);
            CHECK_NPV(true, 3.0, no_discount, leg, today);

            // override
            settings.includeTodaysCashFlows = false;

            CHECK_NPV(false, 2.0, no_discount, leg, today);
            CHECK_NPV(true, 2.0, no_discount, leg, today);
         }
      }

      [Fact]
      public void testAccessViolation()
      {
         // Testing dynamic cast of coupon in Black pricer...

         using (Settings settings = new Settings())
         {
            Date todaysDate = new Date(7, Month.April, 2010);
            Date settlementDate = new Date(9, Month.April, 2010);
            settings.setEvaluationDate(todaysDate);
            Calendar calendar = new TARGET();

            Handle<YieldTermStructure> rhTermStructure = new Handle<YieldTermStructure>(
               Utilities.flatRate(settings, settlementDate, 0.04875825, new Actual365Fixed()));

            double volatility = 0.10;
            Handle<OptionletVolatilityStructure> vol = new Handle<OptionletVolatilityStructure>(
               new ConstantOptionletVolatility(settings, 
                                               2,
                                               calendar,
                                               BusinessDayConvention.ModifiedFollowing,
                                               volatility,
                                               new Actual365Fixed()));

            IborIndex index3m = new USDLibor(new Period(3, TimeUnit.Months), settings, rhTermStructure);

            Date payDate = new Date(20, Month.December, 2013);
            Date startDate = new Date(20, Month.September, 2013);
            Date endDate = new Date(20, Month.December, 2013);
            double spread = 0.0115;
            IborCouponPricer pricer = new BlackIborCouponPricer(vol);
            FloatingRateCoupon coupon = new FloatingRateCoupon(payDate, 100, startDate, endDate, 2,
                                                               index3m, 1.0, spread / 100);
            coupon.setPricer(pricer);

            try
            {
               // this caused an access violation in version 1.0
               coupon.amount();
            }
            catch (Exception)
            {
               // ok; proper exception thrown
            }
         }
      }

      [Fact]
      public void testDefaultSettlementDate()
      {
         // Testing default evaluation date in cashflows methods...
         Settings settings = new Settings();
         Date today = settings.evaluationDate();
         Schedule schedule = new
         MakeSchedule(settings)
         .from(today - new Period(2, TimeUnit.Months)).to(today + new Period(4, TimeUnit.Months))
         .withFrequency(Frequency.Semiannual)
         .withCalendar(new TARGET())
         .withConvention(BusinessDayConvention.Unadjusted)
         .backwards().value();

         List<CashFlow> leg = new FixedRateLeg(schedule)
         .withCouponRates(0.03, new Actual360())
         .withPaymentCalendar(new TARGET())
         .withNotionals(100.0)
         .withPaymentAdjustment(BusinessDayConvention.Following);

         double accruedPeriod = CashFlows.accruedPeriod(leg, false);
         if (accruedPeriod == 0.0)
            QAssert.Fail("null accrued period with default settlement date");

         int accruedDays = CashFlows.accruedDays(leg, false);
         if (accruedDays == 0)
            QAssert.Fail("no accrued days with default settlement date");

         double accruedAmount = CashFlows.accruedAmount(leg, false);
         if (accruedAmount == 0.0)
            QAssert.Fail("null accrued amount with default settlement date");
      }

      [Fact]
      public void testNullFixingDays()
      {
         // Testing ibor leg construction with null fixing days...
         Settings settings = new Settings();
         Date today = settings.evaluationDate();
         Schedule schedule = new
         MakeSchedule(settings)
         .from(today - new Period(2, TimeUnit.Months)).to(today + new Period(4, TimeUnit.Months))
         .withFrequency(Frequency.Semiannual)
         .withCalendar(new TARGET())
         .withConvention(BusinessDayConvention.Following)
         .backwards().value();

         IborIndex index = new USDLibor(new Period(6, TimeUnit.Months), settings);
         List<CashFlow> leg = new IborLeg(schedule, index)
         // this can happen with default values, and caused an
         // exception when the null was not managed properly
         .withFixingDays(null)
         .withNotionals(100.0);
      }
   }
}
