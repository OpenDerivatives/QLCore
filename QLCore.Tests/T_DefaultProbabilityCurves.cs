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

using System;
using System.Collections.Generic;
using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_DefaultProbabilityCurves
   {
      [Fact]
      public void testDefaultProbability()
      {
         // Testing default-probability structure...

         double hazardRate = 0.0100;
         Handle<Quote> hazardRateQuote = new Handle<Quote>(new SimpleQuote(hazardRate));
         DayCounter dayCounter = new Actual360();
         Calendar calendar = new TARGET();
         int n = 20;

         double tolerance = 1.0e-10;
         Date today = Settings.Instance.evaluationDate();
         Date startDate = today;
         Date endDate = startDate;

         FlatHazardRate flatHazardRate = new FlatHazardRate(startDate, hazardRateQuote, dayCounter);

         for (int i = 0; i < n; i++)
         {
            startDate = endDate;
            endDate = calendar.advance(endDate, 1, TimeUnit.Years);

            double pStart = flatHazardRate.defaultProbability(startDate);
            double pEnd = flatHazardRate.defaultProbability(endDate);

            double pBetweenComputed =
               flatHazardRate.defaultProbability(startDate, endDate);

            double pBetween = pEnd - pStart;

            if (Math.Abs(pBetween - pBetweenComputed) > tolerance)
               QAssert.Fail("Failed to reproduce probability(d1, d2) "
                            + "for default probability structure\n"
                            + "    calculated probability: " + pBetweenComputed + "\n"
                            + "    expected probability:   " + pBetween);

            double t2 = dayCounter.yearFraction(today, endDate);
            double timeProbability = flatHazardRate.defaultProbability(t2);
            double dateProbability =
               flatHazardRate.defaultProbability(endDate);

            if (Math.Abs(timeProbability - dateProbability) > tolerance)
               QAssert.Fail("single-time probability and single-date probability do not match\n"
                            + "    time probability: " + timeProbability + "\n"
                            + "    date probability: " + dateProbability);

            double t1 = dayCounter.yearFraction(today, startDate);
            timeProbability = flatHazardRate.defaultProbability(t1, t2);
            dateProbability = flatHazardRate.defaultProbability(startDate, endDate);

            if (Math.Abs(timeProbability - dateProbability) > tolerance)
               QAssert.Fail("double-time probability and double-date probability do not match\n"
                            + "    time probability: " + timeProbability + "\n"
                            + "    date probability: " + dateProbability);

         }
      }

      [Fact]
      public void testFlatHazardRate()
      {

         // Testing flat hazard rate...

         double hazardRate = 0.0100;
         Handle<Quote> hazardRateQuote = new Handle<Quote>(new SimpleQuote(hazardRate));
         DayCounter dayCounter = new Actual360();
         Calendar calendar = new TARGET();
         int n = 20;

         double tolerance = 1.0e-10;
         Date today = Settings.Instance.evaluationDate();
         Date startDate = today;
         Date endDate = startDate;

         FlatHazardRate flatHazardRate = new FlatHazardRate(today, hazardRateQuote, dayCounter);

         for (int i = 0; i < n; i++)
         {
            endDate = calendar.advance(endDate, 1, TimeUnit.Years);
            double t = dayCounter.yearFraction(startDate, endDate);
            double probability = 1.0 - Math.Exp(-hazardRate * t);
            double computedProbability = flatHazardRate.defaultProbability(t);

            if (Math.Abs(probability - computedProbability) > tolerance)
               QAssert.Fail("Failed to reproduce probability for flat hazard rate\n"
                            + "    calculated probability: " + computedProbability + "\n"
                            + "    expected probability:   " + probability);
         }
      }

      [Fact]
      public void testFlatHazardConsistency() 
      {
         // Testing piecewise-flat hazard-rate consistency...
         testBootstrapFromSpread<HazardRate,BackwardFlat>();
         testBootstrapFromUpfront<HazardRate,BackwardFlat>();
      }

      [Fact]
      public void testFlatDensityConsistency()
      {
         // Testing piecewise-flat default-density consistency...
         testBootstrapFromSpread<DefaultDensity, BackwardFlat>();
         testBootstrapFromUpfront<DefaultDensity, BackwardFlat>();
      }

      [Fact]
      public void testLinearDensityConsistency()
      {
         // Testing piecewise-linear default-density consistency...
         testBootstrapFromSpread<DefaultDensity, Linear>();
         testBootstrapFromUpfront<DefaultDensity, Linear>();
      }

      [Fact]
      public void testLogLinearSurvivalConsistency()
      {
         // Testing log-linear survival-probability consistency...
         testBootstrapFromSpread<SurvivalProbability, LogLinear>();
         testBootstrapFromUpfront<SurvivalProbability, LogLinear>();
      }

      [Fact]
      public void testSingleInstrumentBootstrap() 
      {
         //Testing single-instrument curve bootstrap...

         Calendar calendar = new TARGET();

         Date today = Settings.Instance.evaluationDate();

         int settlementDays = 0;

         double quote = 0.005;
         Period tenor = new Period(2, TimeUnit.Years);

         Frequency frequency = Frequency.Quarterly;
         BusinessDayConvention convention = BusinessDayConvention.Following;
         DateGeneration.Rule rule = DateGeneration.Rule.TwentiethIMM;
         DayCounter dayCounter = new Thirty360();
         double recoveryRate = 0.4;

         RelinkableHandle<YieldTermStructure> discountCurve = new RelinkableHandle<YieldTermStructure>();
         discountCurve.linkTo(new FlatForward(today,0.06,new Actual360()));

         List<CdsHelper> helpers = new List<CdsHelper>();

         helpers.Add(
                     new SpreadCdsHelper(quote, tenor,
                                       settlementDays, calendar,
                                       frequency, convention, rule,
                                       dayCounter, recoveryRate,
                                       discountCurve));

         PiecewiseDefaultCurve<HazardRate,BackwardFlat> defaultCurve = new PiecewiseDefaultCurve<HazardRate,BackwardFlat>(today, helpers,
                                                                     dayCounter);
         defaultCurve.recalculate();
      }

      [Fact]
      public void testUpfrontBootstrap() {
         //Testing bootstrap on upfront quotes...

         SavedSettings backup = new SavedSettings();
         // not taken into account, this would prevent the upfront from being used
         Settings.Instance.includeTodaysCashFlows = false;

         testBootstrapFromUpfront<HazardRate,BackwardFlat>();

         // also ensure that we didn't override the flag permanently
         bool? flag = Settings.Instance.includeTodaysCashFlows;
         if (flag != false)
               QAssert.Fail("Cash-flow settings improperly modified");
      }

      [Fact]
      /* This test attempts to build a default curve from CDS spreads as of 1 Apr 2020. The spreads are real and from a 
         distressed reference entity with an inverted CDS spread curve. Using the default IterativeBootstrap with no 
         retries, the default curve building fails. Allowing retries, it expands the min survival probability bounds but 
         still fails. We set dontThrow to true in IterativeBootstrap to use a fall back curve.
      */
      public void testIterativeBootstrapRetries() {

         //Testing iterative bootstrap with retries...

         SavedSettings backup = new SavedSettings();

         Date asof = new Date(1, Month.Apr, 2020);
         Settings.Instance.setEvaluationDate(asof);
         Actual365Fixed tsDayCounter = new Actual365Fixed();

         // USD discount curve built out of FedFunds OIS swaps.
         List<Date> usdCurveDates = new List<Date>() 
         {
            new Date(1, Month.Apr, 2020),
            new Date(2, Month.Apr, 2020),
            new Date(14, Month.Apr, 2020),
            new Date(21, Month.Apr, 2020),
            new Date(28, Month.Apr, 2020),
            new Date(6, Month.May, 2020),
            new Date(5, Month.Jun, 2020),
            new Date(7, Month.Jul, 2020),
            new Date(5, Month.Aug, 2020),
            new Date(8, Month.Sep, 2020),
            new Date(7, Month.Oct, 2020),
            new Date(5, Month.Nov, 2020),
            new Date(7, Month.Dec, 2020),
            new Date(6, Month.Jan, 2021),
            new Date(5, Month.Feb, 2021),
            new Date(5, Month.Mar, 2021),
            new Date(7, Month.Apr, 2021),
            new Date(4, Month.Apr, 2022),
            new Date(3, Month.Apr, 2023),
            new Date(3, Month.Apr, 2024),
            new Date(3, Month.Apr, 2025),
            new Date(5, Month.Apr, 2027),
            new Date(3, Month.Apr, 2030),
            new Date(3, Month.Apr, 2035),
            new Date(3, Month.Apr, 2040),
            new Date(4, Month.Apr, 2050)
         };

         List<double> usdCurveDfs = new List<double>()
         {
            1.000000000,
            0.999955835,
            0.999931070,
            0.999914629,
            0.999902799,
            0.999887990,
            0.999825782,
            0.999764392,
            0.999709076,
            0.999647785,
            0.999594638,
            0.999536198,
            0.999483093,
            0.999419291,
            0.999379417,
            0.999324981,
            0.999262356,
            0.999575101,
            0.996135441,
            0.995228348,
            0.989366687,
            0.979271200,
            0.961150726,
            0.926265361,
            0.891640651,
            0.839314063
         };

         Handle<YieldTermStructure> usdYts = new Handle<YieldTermStructure>(
                                                new InterpolatedDiscountCurve<LogLinear>(
                                                   usdCurveDates, usdCurveDfs, tsDayCounter));

         // CDS spreads
         Dictionary<Period, double> cdsSpreads = new Dictionary<Period, double>();
         cdsSpreads.Add(new Period(6, TimeUnit.Months), 2.957980250);
         cdsSpreads.Add(new Period(1, TimeUnit.Years), 3.076933100);
         cdsSpreads.Add(new Period(2, TimeUnit.Years), 2.944524520);
         cdsSpreads.Add(new Period(3, TimeUnit.Years), 2.844498960);
         cdsSpreads.Add(new Period(4, TimeUnit.Years), 2.769234420);
         cdsSpreads.Add(new Period(5, TimeUnit.Years), 2.713474100);
         double recoveryRate = 0.035;

         // Conventions
         int settlementDays = 1;
         WeekendsOnly calendar = new WeekendsOnly();
         Frequency frequency = Frequency.Quarterly;
         BusinessDayConvention paymentConvention = BusinessDayConvention.Following;
         DateGeneration.Rule rule = DateGeneration.Rule.CDS2015;
         Actual360 dayCounter = new Actual360();
         Actual360 lastPeriodDayCounter = new Actual360(true);

         // Create the CDS spread helpers.
         List<CdsHelper> instruments = new List<CdsHelper>();
         foreach (KeyValuePair<Period, double> it in cdsSpreads) 
         {
            instruments.Add(
                  new SpreadCdsHelper(it.Value, it.Key, settlementDays, calendar,
                                       frequency, paymentConvention, rule, dayCounter, recoveryRate, usdYts, true, true, null,
                                       lastPeriodDayCounter));
         }

         // Create the default curve with the default IterativeBootstrap.
         DefaultProbabilityTermStructure dpts = new PiecewiseDefaultCurve<SurvivalProbability, LogLinear, IterativeBootstrapForCds>(asof, instruments, tsDayCounter);

         // Check that the default curve throws by requesting a default probability.
         Date testDate = new Date(21, Month.Dec, 2020);
         try 
         { 
            dpts.survivalProbability(testDate);
            throw new Exception();
         }
         catch {}

         // Create the default curve with an IterativeBootstrap allowing for 4 retries.
         // Use a maxFactor value of 1.0 so that we still use the previous survival probability at each pillar. In other
         // words, the survival probability cannot increase with time so best max at current pillar is the previous 
         // pillar's value - there is no point increasing it on a retry.
         IterativeBootstrap<PiecewiseDefaultCurve, DefaultProbabilityTermStructure> ib =
            new IterativeBootstrap<PiecewiseDefaultCurve, DefaultProbabilityTermStructure>(null, null, null, 5, 1.0, 10.0);

         dpts = new PiecewiseDefaultCurve<SurvivalProbability, LogLinear>(asof, instruments, tsDayCounter, ib);

         // Check that the default curve still throws. It throws at the third pillar because the survival probability is 
         // too low at the second pillar.
         try 
         { 
            dpts.survivalProbability(testDate);
            throw new Exception();
         }
         catch {}
         
         // Create the default curve with an IterativeBootstrap that allows for 4 retries and does not throw.
         IterativeBootstrap<PiecewiseDefaultCurve, DefaultProbabilityTermStructure> ibNoThrow =
            new IterativeBootstrap<PiecewiseDefaultCurve, DefaultProbabilityTermStructure>(null, null, null, 5, 1.0, 10.0, true, 2);

         dpts = new PiecewiseDefaultCurve<SurvivalProbability, LogLinear>(asof, instruments, tsDayCounter, ibNoThrow);

         try 
         { 
            dpts.survivalProbability(testDate);
         }
         catch 
         {
            throw new Exception();
         }
      }

      void testBootstrapFromSpread<T, I>() 
            where T : ITraits<DefaultProbabilityTermStructure>, new()
            where I : class, IInterpolationFactory, new()
      {
         Calendar calendar = new TARGET();

         Date today = Settings.Instance.evaluationDate();

         int settlementDays = 1;

         List<double> quote = new List<double>();
         quote.Add(0.005);
         quote.Add(0.006);
         quote.Add(0.007);
         quote.Add(0.009);

         List<int> n = new List<int>();
         n.Add(1);
         n.Add(2);
         n.Add(3);
         n.Add(5);

         Frequency frequency = Frequency.Quarterly;
         BusinessDayConvention convention = BusinessDayConvention.Following;
         DateGeneration.Rule rule = DateGeneration.Rule.TwentiethIMM;
         DayCounter dayCounter = new Thirty360();
         double recoveryRate = 0.4;

         RelinkableHandle<YieldTermStructure> discountCurve = new RelinkableHandle<YieldTermStructure>();
         discountCurve.linkTo(new FlatForward(today,0.06,new Actual360()));

         List<CdsHelper> helpers = new List<CdsHelper>();

         for(int i=0; i<n.Count; i++)
               helpers.Add(
                           new SpreadCdsHelper(quote[i], new Period(n[i], TimeUnit.Years),
                                          settlementDays, calendar,
                                          frequency, convention, rule,
                                          dayCounter, recoveryRate,
                                          discountCurve));

         RelinkableHandle<DefaultProbabilityTermStructure> piecewiseCurve = new RelinkableHandle<DefaultProbabilityTermStructure>();
         piecewiseCurve.linkTo(
                  new PiecewiseDefaultCurve<T,I>(today, helpers,
                                                   new Thirty360()));

         double notional = 1.0;
         double tolerance = 1.0e-6;

         // ensure apple-to-apple comparison
         SavedSettings backup = new SavedSettings();
         Settings.Instance.includeTodaysCashFlows = true;

         for (int i=0; i<n.Count; i++) {
               Date protectionStart = today + settlementDays;
               Date startDate = calendar.adjust(protectionStart, convention);
               Date endDate = today + new Period(n[i], TimeUnit.Years);

               Schedule schedule = new Schedule(startDate, endDate, new Period(frequency), calendar,
                                 convention, BusinessDayConvention.Unadjusted, rule, false);

               CreditDefaultSwap cds = new CreditDefaultSwap(CreditDefaultSwap.Protection.Side.Buyer, notional, quote[i],
                                                         schedule, convention, dayCounter,
                                                         true, true, protectionStart);
               cds.setPricingEngine(new MidPointCdsEngine(piecewiseCurve, recoveryRate,
                                                         discountCurve));

               // test
               double inputRate = quote[i];
               double computedRate = cds.fairSpread();
               if (Math.Abs(inputRate - computedRate) > tolerance)
                  QAssert.Fail(
                     "\nFailed to reproduce fair spread for " + n[i] +
                     "Y credit-default swaps\n"
                     + "    computed rate: " + computedRate.ToString() + "\n"
                     + "    input rate:    " + inputRate.ToString());
         }
      }

      void testBootstrapFromUpfront<T, I>() 
            where T : ITraits<DefaultProbabilityTermStructure>, new()
            where I : class, IInterpolationFactory, new()
      {

         Calendar calendar = new TARGET();

         Date today = Settings.Instance.evaluationDate();

         int settlementDays = 1;

         List<double> quote = new List<double>();
         quote.Add(0.01);
         quote.Add(0.02);
         quote.Add(0.04);
         quote.Add(0.06);

         List<int> n = new List<int>();
         n.Add(2);
         n.Add(3);
         n.Add(5);
         n.Add(7);

         double fixedRate = 0.05;
         Frequency frequency = Frequency.Quarterly;
         BusinessDayConvention convention = BusinessDayConvention.ModifiedFollowing;
         DateGeneration.Rule rule = DateGeneration.Rule.CDS;
         DayCounter dayCounter = new Actual360();
         double recoveryRate = 0.4;
         int upfrontSettlementDays = 3;

         RelinkableHandle<YieldTermStructure> discountCurve = new RelinkableHandle<YieldTermStructure>();
         discountCurve.linkTo(new FlatForward(today,0.06,new Actual360()));

         List<CdsHelper> helpers = new List<CdsHelper>();

         for(int i=0; i<n.Count; i++)
               helpers.Add(
                     new UpfrontCdsHelper(quote[i], fixedRate,
                                          new Period(n[i], TimeUnit.Years),
                                          settlementDays, calendar,
                                          frequency, convention, rule,
                                          dayCounter, recoveryRate,
                                          discountCurve,
                                          upfrontSettlementDays, 
                                          true, true, null, new Actual360(true)));

         RelinkableHandle<DefaultProbabilityTermStructure> piecewiseCurve = new RelinkableHandle<DefaultProbabilityTermStructure>();
         piecewiseCurve.linkTo(new PiecewiseDefaultCurve<T,I>(today, helpers, new Thirty360()));

         double notional = 1.0;
         double tolerance = 1.0e-6;

         SavedSettings backup = new SavedSettings();
         // ensure apple-to-apple comparison
         Settings.Instance.includeTodaysCashFlows = true;

         for (int i=0; i<n.Count; i++) 
         {
            Date protectionStart = today + settlementDays;
            Date startDate = calendar.adjust(protectionStart, convention);
            Date endDate = today + new Period(n[i], TimeUnit.Years);
            Date upfrontDate = calendar.advance(today,
                                       upfrontSettlementDays,
                                       TimeUnit.Days,
                                       convention);

            Schedule schedule = new Schedule(startDate, endDate, new Period(frequency), calendar,
                                             convention, BusinessDayConvention.Unadjusted, rule, false);
            
            schedule.isRegular().Insert(0, schedule.isRegular()[0]);
            schedule.dates().Insert(0, protectionStart);

            CreditDefaultSwap cds = new CreditDefaultSwap(CreditDefaultSwap.Protection.Side.Buyer, notional,
                                                         quote[i], fixedRate,
                                                         schedule, convention, dayCounter,
                                                         true, true, protectionStart,
                                                         upfrontDate,
                                                         null,
                                                         new Actual360(true),
                                                         true);

            cds.setPricingEngine(new MidPointCdsEngine(piecewiseCurve, recoveryRate,
                                                       discountCurve, true));

            // test
            double inputUpfront = quote[i];
            double computedUpfront = cds.fairUpfront();
            if (Math.Abs(inputUpfront - computedUpfront) > tolerance)
               QAssert.Fail(
                  "\nFailed to reproduce fair upfront for " + n[i] +
                  "Y credit-default swaps\n"
                  + "    computed: " + computedUpfront.ToString() + "\n"
                  + "    expected: " + inputUpfront.ToString());
         }

         backup.Dispose();
      }
   }
}
