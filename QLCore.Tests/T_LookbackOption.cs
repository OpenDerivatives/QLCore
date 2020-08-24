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
using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_LookbackOption
   {
      void REPORT_FAILURE_FLOATING(string greekName,
                                   double minmax,
                                   FloatingTypePayoff payoff,
                                   Exercise exercise,
                                   double s,
                                   double q,
                                   double r,
                                   Date today,
                                   double v,
                                   double expected,
                                   double calculated,
                                   double error,
                                   double tolerance)
      {
         QAssert.Fail(exercise.GetType() + " "
                      + payoff.optionType() + " lookback option with "
                      + payoff + " payoff:\n"
                      + "    underlying value  " + s + "\n"
                      + "    dividend yield:   " + q + "\n"
                      + "    risk-free rate:   " + r + "\n"
                      + "    reference date:   " + today + "\n"
                      + "    maturity:         " + exercise.lastDate() + "\n"
                      + "    volatility:       " + v + "\n\n"
                      + "    expected " + greekName + ":   " + expected + "\n"
                      + "    calculated " + greekName + ": " + calculated + "\n"
                      + "    error:            " + error + "\n"
                      + "    tolerance:        " + tolerance);
      }

      void REPORT_FAILURE_FIXED(string greekName,
                                double minmax,
                                StrikedTypePayoff payoff,
                                Exercise exercise,
                                double s,
                                double q,
                                double r,
                                Date today,
                                double v,
                                double expected,
                                double calculated,
                                double error,
                                double tolerance)
      {
         QAssert.Fail(exercise.GetType() + " "
                      + payoff.optionType() + " lookback option with "
                      + payoff + " payoff:\n"
                      + "    underlying value  " + s + "\n"
                      + "    strike:           " + payoff.strike() + "\n"
                      + "    dividend yield:   " + q + "\n"
                      + "    risk-free rate:   " + r + "\n"
                      + "    reference date:   " + today + "\n"
                      + "    maturity:         " + exercise.lastDate() + "\n"
                      + "    volatility:       " + v + "\n\n"
                      + "    expected " + greekName + ":   " + expected + "\n"
                      + "    calculated " + greekName + ": " + calculated + "\n"
                      + "    error:            " + error + "\n"
                      + "    tolerance:        " + tolerance);
      }

      void REPORT_FAILURE_MC(string lookbackType,
                             Option.Type optionType,
                             double analytical,
                             double monteCarlo,
                             double tolerance)
      {
         QAssert.Fail("Analytical and MC " + lookbackType + " " + optionType.ToString() + " values differed by more than tolerance" + "\n"
                     + "    Analytical:    " + analytical + "\n"
                     + "    Monte Carlo:   " + monteCarlo + "\n"
                     + "    tolerance:     " + tolerance + "\n"
                     + "    difference:    " + Math.Abs(analytical - monteCarlo));
      }

      class LookbackOptionData
      {
         public LookbackOptionData(Option.Type type_, double strike_, double minmax_, double s_, double q_, double r_,
                                   double t_, double v_, double l_, double t1_, double result_, double tol_)
         {
            type = type_;
            strike = strike_;
            minmax = minmax_;
            s = s_;
            q = q_;
            r = r_;
            t = t_;
            v = v_;
            l = l_;
            t1 = t1_;
            result = result_;
            tol = tol_;
         }
         public Option.Type type;
         public double strike;
         public double minmax;
         public double s;        // spot
         public double q;        // dividend
         public double r;        // risk-free rate
         public double t;        // time to maturity
         public double v;  // volatility

         //Partial-time lookback options:
         public double l;        // level above/below actual extremum
         public double t1;       // time to start of lookback period

         public double result;   // result
         public double tol;      // tolerance
      }

      [Fact]
      public void testAnalyticContinuousFloatingLookback()
      {
         // Testing analytic continuous floating-strike lookback options
         LookbackOptionData[] values =
         {
            // data from "Option Pricing Formulas", Haug, 1998, pg.61-62
            new LookbackOptionData(Option.Type.Call, 0, 100, 120.0, 0.06, 0.10, 0.50, 0.30, 0, 0, 25.3533, 1.0e-4),
            // data from "Connecting discrete and continuous path-dependent options",
            // Broadie, Glasserman & Kou, 1999, pg.70-74
            new LookbackOptionData(Option.Type.Call, 0, 100, 100.0, 0.00, 0.05, 1.00, 0.30, 0, 0, 23.7884, 1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0, 100, 100.0, 0.00, 0.05, 0.20, 0.30, 0, 0, 10.7190, 1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0, 100, 110.0, 0.00, 0.05, 0.20, 0.30, 0, 0, 14.4597, 1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0, 100, 100.0, 0.00, 0.10, 0.50, 0.30, 0, 0, 15.3526, 1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0, 110, 100.0, 0.00, 0.10, 0.50, 0.30, 0, 0, 16.8468, 1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0, 120, 100.0, 0.00, 0.10, 0.50, 0.30, 0, 0, 21.0645, 1.0e-4),
         };

         DayCounter dc = new Actual360();
         Date today = Date.Today;

         SimpleQuote spot = new SimpleQuote(0.0);
         SimpleQuote qRate = new SimpleQuote(0.0);
         YieldTermStructure qTS = Utilities.flatRate(today, qRate, dc);
         SimpleQuote rRate = new SimpleQuote(0.0);
         YieldTermStructure rTS = Utilities.flatRate(today, rRate, dc);
         SimpleQuote vol = new SimpleQuote(0.0);
         BlackVolTermStructure volTS = Utilities.flatVol(today, vol, dc);

         for (int i = 0; i < values.Length; i++)
         {
            Date exDate = today + Convert.ToInt32(values[i].t * 360 + 0.5);
            Exercise exercise = new EuropeanExercise(exDate);

            spot .setValue(values[i].s);
            qRate.setValue(values[i].q);
            rRate.setValue(values[i].r);
            vol  .setValue(values[i].v);

            FloatingTypePayoff payoff = new FloatingTypePayoff(values[i].type);

            BlackScholesMertonProcess stochProcess = new BlackScholesMertonProcess(
               new Handle<Quote>(spot),
               new Handle<YieldTermStructure>(qTS),
               new Handle<YieldTermStructure>(rTS),
               new Handle<BlackVolTermStructure>(volTS));

            IPricingEngine engine = new AnalyticContinuousFloatingLookbackEngine(stochProcess);

            ContinuousFloatingLookbackOption option = new ContinuousFloatingLookbackOption(values[i].minmax,
                                                                                           payoff,
                                                                                           exercise);
            option.setPricingEngine(engine);

            double calculated = option.NPV();
            double expected = values[i].result;
            double error = Math.Abs(calculated - expected);
            if (error > values[i].tol)
            {
               REPORT_FAILURE_FLOATING("value", values[i].minmax, payoff,
                                       exercise, values[i].s, values[i].q,
                                       values[i].r, today, values[i].v,
                                       expected, calculated, error,
                                       values[i].tol);
            }
         }
      }

      [Fact]
      public void testAnalyticContinuousFixedLookback()
      {
         // Testing analytic continuous fixed-strike lookback options
         LookbackOptionData[] values =
         {
            // data from "Option Pricing Formulas", Haug, 1998, pg.63-64
            //type,            strike, minmax,  s,     q,    r,    t,    v,    l, t1, result,  tol
            new LookbackOptionData(Option.Type.Call,    95,     100,     100.0, 0.00, 0.10, 0.50, 0.10, 0, 0,  13.2687, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    95,     100,     100.0, 0.00, 0.10, 0.50, 0.20, 0, 0,  18.9263, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    95,     100,     100.0, 0.00, 0.10, 0.50, 0.30, 0, 0,  24.9857, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    100,    100,     100.0, 0.00, 0.10, 0.50, 0.10, 0, 0,   8.5126, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    100,    100,     100.0, 0.00, 0.10, 0.50, 0.20, 0, 0,  14.1702, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    100,    100,     100.0, 0.00, 0.10, 0.50, 0.30, 0, 0,  20.2296, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    105,    100,     100.0, 0.00, 0.10, 0.50, 0.10, 0, 0,   4.3908, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    105,    100,     100.0, 0.00, 0.10, 0.50, 0.20, 0, 0,   9.8905, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    105,    100,     100.0, 0.00, 0.10, 0.50, 0.30, 0, 0,  15.8512, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    95,     100,     100.0, 0.00, 0.10, 1.00, 0.10, 0, 0,  18.3241, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    95,     100,     100.0, 0.00, 0.10, 1.00, 0.20, 0, 0,  26.0731, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    95,     100,     100.0, 0.00, 0.10, 1.00, 0.30, 0, 0,  34.7116, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    100,    100,     100.0, 0.00, 0.10, 1.00, 0.10, 0, 0,  13.8000, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    100,    100,     100.0, 0.00, 0.10, 1.00, 0.20, 0, 0,  21.5489, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    100,    100,     100.0, 0.00, 0.10, 1.00, 0.30, 0, 0,  30.1874, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    105,    100,     100.0, 0.00, 0.10, 1.00, 0.10, 0, 0,   9.5445, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    105,    100,     100.0, 0.00, 0.10, 1.00, 0.20, 0, 0,  17.2965, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    105,    100,     100.0, 0.00, 0.10, 1.00, 0.30, 0, 0,  25.9002, 1.0e-4),

            new LookbackOptionData(Option.Type.Put,    95,     100,     100.0, 0.00, 0.10, 0.50, 0.10, 0, 0,   0.6899, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    95,     100,     100.0, 0.00, 0.10, 0.50, 0.20, 0, 0,   4.4448, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    95,     100,     100.0, 0.00, 0.10, 0.50, 0.30, 0, 0,   8.9213, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    100,    100,     100.0, 0.00, 0.10, 0.50, 0.10, 0, 0,   3.3917, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    100,    100,     100.0, 0.00, 0.10, 0.50, 0.20, 0, 0,   8.3177, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    100,    100,     100.0, 0.00, 0.10, 0.50, 0.30, 0, 0,  13.1579, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    105,    100,     100.0, 0.00, 0.10, 0.50, 0.10, 0, 0,   8.1478, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    105,    100,     100.0, 0.00, 0.10, 0.50, 0.20, 0, 0,  13.0739, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    105,    100,     100.0, 0.00, 0.10, 0.50, 0.30, 0, 0,  17.9140, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    95,     100,     100.0, 0.00, 0.10, 1.00, 0.10, 0, 0,   1.0534, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    95,     100,     100.0, 0.00, 0.10, 1.00, 0.20, 0, 0,   6.2813, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    95,     100,     100.0, 0.00, 0.10, 1.00, 0.30, 0, 0,  12.2376, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    100,    100,     100.0, 0.00, 0.10, 1.00, 0.10, 0, 0,   3.8079, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    100,    100,     100.0, 0.00, 0.10, 1.00, 0.20, 0, 0,  10.1294, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    100,    100,     100.0, 0.00, 0.10, 1.00, 0.30, 0, 0,  16.3889, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    105,    100,     100.0, 0.00, 0.10, 1.00, 0.10, 0, 0,   8.3321, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    105,    100,     100.0, 0.00, 0.10, 1.00, 0.20, 0, 0,  14.6536, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,    105,    100,     100.0, 0.00, 0.10, 1.00, 0.30, 0, 0,  20.9130, 1.0e-4)

         };

         DayCounter dc = new Actual360();
         Date today = Date.Today;

         SimpleQuote spot = new SimpleQuote(0.0);
         SimpleQuote qRate = new SimpleQuote(0.0);
         YieldTermStructure qTS = Utilities.flatRate(today, qRate, dc);
         SimpleQuote rRate = new SimpleQuote(0.0);
         YieldTermStructure rTS = Utilities.flatRate(today, rRate, dc);
         SimpleQuote vol = new SimpleQuote(0.0);
         BlackVolTermStructure volTS = Utilities.flatVol(today, vol, dc);

         for (int i = 0; i < values.Length; i++)
         {
            Date exDate = today + Convert.ToInt32(values[i].t * 360 + 0.5);
            Exercise exercise = new EuropeanExercise(exDate);

            spot .setValue(values[i].s);
            qRate.setValue(values[i].q);
            rRate.setValue(values[i].r);
            vol  .setValue(values[i].v);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(values[i].type, values[i].strike);

            BlackScholesMertonProcess stochProcess = new BlackScholesMertonProcess(
               new Handle<Quote>(spot),
               new Handle<YieldTermStructure>(qTS),
               new Handle<YieldTermStructure>(rTS),
               new Handle<BlackVolTermStructure>(volTS));

            IPricingEngine engine = new AnalyticContinuousFixedLookbackEngine(stochProcess);

            ContinuousFixedLookbackOption option = new ContinuousFixedLookbackOption(values[i].minmax,
                                                                                     payoff, exercise);
            option.setPricingEngine(engine);

            double calculated = option.NPV();
            double expected = values[i].result;
            double error = Math.Abs(calculated - expected);
            if (error > values[i].tol)
            {
               REPORT_FAILURE_FIXED("value", values[i].minmax, payoff, exercise,
                                    values[i].s, values[i].q, values[i].r, today,
                                    values[i].v, expected, calculated, error,
                                    values[i].tol);
            }
         }

      }

      [Fact]
      public void testAnalyticContinuousPartialFloatingLookback()
      {
         // Testing analytic continuous partial floating-strike lookback options...");
         LookbackOptionData[] values =
         {
            // data from "Option Pricing Formulas, Second Edition", Haug, 2006, pg.146

            //type,         strike, minmax, s,    q,    r,    t,    v,    l,  t1,     result,   tol
            new LookbackOptionData(Option.Type.Call, 0,       90,     90,   0,   0.06, 1,    0.1,  1,  0.25,   8.6524,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,       90,     90,   0,   0.06, 1,    0.1,  1,  0.5,    9.2128,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,       90,     90,   0,   0.06, 1,    0.1,  1,  0.75,   9.5567,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.1,  1,  0.25,  10.5751,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.1,  1,  0.5,   11.2601,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.1,  1,  0.75,  11.6804,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,       90,     90,   0,   0.06, 1,    0.2,  1,  0.25,  13.3402,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,       90,     90,   0,   0.06, 1,    0.2,  1,  0.5,   14.5121,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,       90,     90,   0,   0.06, 1,    0.2,  1,  0.75,  15.314,    1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.2,  1,  0.25,  16.3047,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.2,  1,  0.5,   17.737,    1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.2,  1,  0.75,  18.7171,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      90,      90,    0,  0.06, 1,    0.3,  1,  0.25,  17.9831,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      90,      90,    0,  0.06, 1,    0.3,  1,  0.5,   19.6618,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      90,      90,    0,  0.06, 1,    0.3,  1,  0.75,  20.8493,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.3,  1,  0.25,  21.9793,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.3,  1,  0.5,   24.0311,   1.0e-4),
            new LookbackOptionData(Option.Type.Call, 0,      110,    110,   0,   0.06, 1,    0.3,  1,  0.75,  25.4825,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.1,  1,  0.25,   2.7189,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.1,  1,  0.5,    3.4639,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.1,  1,  0.75,   4.1912,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.1,  1,  0.25,   3.3231,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.1,  1,  0.5,    4.2336,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.1,  1,  0.75,   5.1226,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.2,  1,  0.25,   7.9153,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.2,  1,  0.5,    9.5825,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.2,  1,  0.75,  11.0362,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.2,  1,  0.25,   9.6743,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.2,  1,  0.5,   11.7119,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.2,  1,  0.75,  13.4887,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.3,  1,  0.25,  13.4719,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.3,  1,  0.5,   16.1495,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,       90,      90,   0,   0.06, 1,    0.3,  1,  0.75,  18.4071,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.3,  1,  0.25,  16.4657,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.3,  1,  0.5,   19.7383,   1.0e-4),
            new LookbackOptionData(Option.Type.Put, 0,      110,     110,   0,   0.06, 1,    0.3,  1,  0.75,  22.4976,   1.0e-4)
         };

         DayCounter dc = new Actual360();
         Date today = Date.Today;

         SimpleQuote spot = new SimpleQuote(0.0);
         SimpleQuote qRate = new SimpleQuote(0.0);
         YieldTermStructure qTS = Utilities.flatRate(today, qRate, dc);
         SimpleQuote rRate = new SimpleQuote(0.0);
         YieldTermStructure rTS = Utilities.flatRate(today, rRate, dc);
         SimpleQuote vol = new SimpleQuote(0.0);
         BlackVolTermStructure volTS = Utilities.flatVol(today, vol, dc);

         for (int i = 0; i < values.Length; i++)
         {
            Date exDate = today + Convert.ToInt32(values[i].t * 360 + 0.5);
            Exercise exercise = new EuropeanExercise(exDate);

            spot .setValue(values[i].s);
            qRate.setValue(values[i].q);
            rRate.setValue(values[i].r);
            vol  .setValue(values[i].v);

            FloatingTypePayoff payoff = new FloatingTypePayoff(values[i].type);

            BlackScholesMertonProcess stochProcess = new BlackScholesMertonProcess(
               new Handle<Quote>(spot),
               new Handle<YieldTermStructure>(qTS),
               new Handle<YieldTermStructure>(rTS),
               new Handle<BlackVolTermStructure>(volTS));

            IPricingEngine engine = new AnalyticContinuousPartialFloatingLookbackEngine(stochProcess);

            Date lookbackEnd = today + Convert.ToInt32(values[i].t1 * 360 + 0.5);
            ContinuousPartialFloatingLookbackOption option = new ContinuousPartialFloatingLookbackOption(
               values[i].minmax, values[i].l, lookbackEnd, payoff, exercise);
            option.setPricingEngine(engine);

            double calculated = option.NPV();
            double expected = values[i].result;
            double error = Math.Abs(calculated - expected);
            if (error > values[i].tol)
            {
               REPORT_FAILURE_FLOATING("value", values[i].minmax, payoff,
                                       exercise, values[i].s, values[i].q,
                                       values[i].r, today, values[i].v,
                                       expected, calculated, error,
                                       values[i].tol);
            }
         }
      }

      [Fact]
      public void testAnalyticContinuousPartialFixedLookback()
      {
         // Testing analytic continuous fixed-strike lookback options
         LookbackOptionData[] values =
         {
            // data from "Option Pricing Formulas, Second Edition", Haug, 2006, pg.148
            //type,         strike, minmax, s,    q,    r,    t,    v, l,   t1,  result,   tol
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.1, 0, 0.25,  20.2845, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.1, 0, 0.5,   19.6239, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.1, 0, 0.75,  18.6244, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.1, 0, 0.25,   4.0432, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.1, 0, 0.5,    3.958,  1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.1, 0, 0.75,   3.7015, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.2, 0, 0.25,  27.5385, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.2, 0, 0.5,   25.8126, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.2, 0, 0.75,  23.4957, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.2, 0, 0.25,  11.4895, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.2, 0, 0.5,   10.8995, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.2, 0, 0.75,   9.8244, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.3, 0, 0.25,  35.4578, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.3, 0, 0.5,   32.7172, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,     90, 0,    100,    0, 0.06,    1,  0.3, 0, 0.75,  29.1473, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.3, 0, 0.25,  19.725,  1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.3, 0, 0.5,   18.4025, 1.0e-4),
            new LookbackOptionData(Option.Type.Call,    110, 0,    100,    0, 0.06,    1,  0.3, 0, 0.75,  16.2976, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.1, 0, 0.25,   0.4973, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.1, 0, 0.5,    0.4632, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.1, 0, 0.75,   0.3863, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.1, 0, 0.25,  12.6978, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.1, 0, 0.5,   10.9492, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.1, 0, 0.75,   9.1555, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.2, 0, 0.25,   4.5863, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.2, 0, 0.5,    4.1925, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.2, 0, 0.75,   3.5831, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.2, 0, 0.25,  19.0255, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.2, 0, 0.5,   16.9433, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.2, 0, 0.75,  14.6505, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.3, 0, 0.25,   9.9348, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.3, 0, 0.5,    9.1111, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,      90, 0,    100,    0, 0.06,    1,  0.3, 0, 0.75,   7.9267, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.3, 0, 0.25,  25.2112, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.3, 0, 0.5,   22.8217, 1.0e-4),
            new LookbackOptionData(Option.Type.Put,     110, 0,    100,    0, 0.06,    1,  0.3, 0, 0.75,  20.0566, 1.0e-4)
         };

         DayCounter dc = new Actual360();
         Date today = Date.Today;

         SimpleQuote spot = new SimpleQuote(0.0);
         SimpleQuote qRate = new SimpleQuote(0.0);
         YieldTermStructure qTS = Utilities.flatRate(today, qRate, dc);
         SimpleQuote rRate = new SimpleQuote(0.0);
         YieldTermStructure rTS = Utilities.flatRate(today, rRate, dc);
         SimpleQuote vol = new SimpleQuote(0.0);
         BlackVolTermStructure volTS = Utilities.flatVol(today, vol, dc);

         for (int i = 0; i < values.Length; i++)
         {
            Date exDate = today + Convert.ToInt32(values[i].t * 360 + 0.5);
            Exercise exercise = new EuropeanExercise(exDate);

            spot .setValue(values[i].s);
            qRate.setValue(values[i].q);
            rRate.setValue(values[i].r);
            vol  .setValue(values[i].v);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(values[i].type, values[i].strike);

            BlackScholesMertonProcess stochProcess = new BlackScholesMertonProcess(
               new Handle<Quote>(spot),
               new Handle<YieldTermStructure>(qTS),
               new Handle<YieldTermStructure>(rTS),
               new Handle<BlackVolTermStructure>(volTS));

            IPricingEngine engine = new AnalyticContinuousPartialFixedLookbackEngine(stochProcess);

            Date lookbackStart = today + Convert.ToInt32(values[i].t1 * 360 + 0.5);
            ContinuousPartialFixedLookbackOption option = new ContinuousPartialFixedLookbackOption(lookbackStart,
                  payoff, exercise);
            option.setPricingEngine(engine);

            double calculated = option.NPV();
            double expected = values[i].result;
            double error = Math.Abs(calculated - expected);
            if (error > values[i].tol)
            {
               REPORT_FAILURE_FIXED("value", values[i].minmax, payoff, exercise,
                                    values[i].s, values[i].q, values[i].r, today,
                                    values[i].v, expected, calculated, error,
                                    values[i].tol);
            }
         }
      }

      [Fact]
      public void testMonteCarloLookback()
      {
         double tolerance = 0.1;

         DayCounter dc = new Actual360();
         Date today = Date.Today;

         double strike = 90;
         double t = 1;
         double t1 = 0.25;

         Date exDate = today + Convert.ToInt32(t * 360 + 0.5);
         Exercise exercise = new EuropeanExercise(exDate);

         SimpleQuote spot = new SimpleQuote(0.0);
         SimpleQuote qRate = new SimpleQuote(0.0);
         YieldTermStructure qTS = Utilities.flatRate(today, qRate, dc);
         SimpleQuote rRate = new SimpleQuote(0.0);
         YieldTermStructure rTS = Utilities.flatRate(today, rRate, dc);
         SimpleQuote vol = new SimpleQuote(0.0);
         BlackVolTermStructure volTS = Utilities.flatVol(today, vol, dc);

         spot.setValue(100);
         qRate.setValue(0);
         rRate.setValue(0.06);
         vol.setValue(0.1);

         BlackScholesMertonProcess stochProcess =
               new BlackScholesMertonProcess(
                  new Handle<Quote>(spot),
                  new Handle<YieldTermStructure>(qTS),
                  new Handle<YieldTermStructure>(rTS),
                  new Handle<BlackVolTermStructure>(volTS));

         Option.Type[] types = new Option.Type[] { Option.Type.Call, Option.Type.Put };

         for (int i = 0; i < types.Length; i++)
         {
            Option.Type type = types[i];

            StrikedTypePayoff payoff = new PlainVanillaPayoff(type, strike);

            /**
            * Partial Fixed
            * **/

            Date lookbackStart = today + Convert.ToInt32(t1 * 360 + 0.5);
            ContinuousPartialFixedLookbackOption partialFixedLookback 
                  = new ContinuousPartialFixedLookbackOption(lookbackStart,
                                                                     payoff,
                                                                     exercise);
            IPricingEngine engine = 
               new AnalyticContinuousPartialFixedLookbackEngine(stochProcess);

            partialFixedLookback.setPricingEngine(engine);

            double analytical = partialFixedLookback.NPV();

            IPricingEngine mcpartialfixedengine =
               new MakeMCLookbackEngine<ContinuousPartialFixedLookbackOption.Arguments, ContinuousPartialFixedLookbackOption.Results,
                                       PseudoRandom, Statistics>(stochProcess)
                                                               .withSteps(2000)
                                                               .withAntitheticVariate()
                                                               .withSeed(1)
                                                               .withAbsoluteTolerance(tolerance)
                                                               .value();

            partialFixedLookback.setPricingEngine(mcpartialfixedengine);
            double monteCarlo = partialFixedLookback.NPV();

            double diff = Math.Abs(analytical - monteCarlo);

            if (diff > tolerance)
            {
               REPORT_FAILURE_MC("Partial Fixed", type, analytical, monteCarlo, tolerance);
            }

            /**
            * Fixed
            * **/

            double minMax = 100;

            ContinuousFixedLookbackOption fixedLookback 
                  = new ContinuousFixedLookbackOption(minMax,
                                                      payoff,
                                                      exercise);
            IPricingEngine analyticalfixedengine = 
                  new AnalyticContinuousFixedLookbackEngine(stochProcess);

            fixedLookback.setPricingEngine(analyticalfixedengine);
            analytical = fixedLookback.NPV();

            IPricingEngine mcfixedengine =
               new MakeMCLookbackEngine<ContinuousFixedLookbackOption.Arguments, ContinuousFixedLookbackOption.Results,
                                       PseudoRandom, Statistics>(stochProcess)
                                                                              .withSteps(2000)
                                                                              .withAntitheticVariate()
                                                                              .withSeed(1)
                                                                              .withAbsoluteTolerance(tolerance)
                                                                              .value();

            fixedLookback.setPricingEngine(mcfixedengine);
            monteCarlo = fixedLookback.NPV();

            diff = Math.Abs(analytical - monteCarlo);

            if (diff > tolerance)
            {
               REPORT_FAILURE_MC("Fixed", type, analytical, monteCarlo, tolerance);
            }

            /**
            * Partial Floating
            * **/

            double lambda = 1;
            Date lookbackEnd = today + Convert.ToInt32(t1 * 360 + 0.5);

            FloatingTypePayoff floatingPayoff = new FloatingTypePayoff(type);

            ContinuousPartialFloatingLookbackOption partialFloating 
                     = new ContinuousPartialFloatingLookbackOption(minMax,
                                                                  lambda,
                                                                  lookbackEnd,
                                                                  floatingPayoff,
                                                                  exercise);
            IPricingEngine analyticalpartialFloatingengine = 
               new AnalyticContinuousPartialFloatingLookbackEngine(stochProcess);
            partialFloating.setPricingEngine(analyticalpartialFloatingengine);

            analytical = partialFloating.NPV();

            IPricingEngine mcpartialfloatingengine =
               new MakeMCLookbackEngine<ContinuousPartialFloatingLookbackOption.Arguments, ContinuousPartialFloatingLookbackOption.Results,
                                       PseudoRandom, Statistics>
               (stochProcess)
               .withSteps(2000)
               .withAntitheticVariate()
               .withSeed(1)
               .withAbsoluteTolerance(tolerance)
               .value();

            partialFloating.setPricingEngine(mcpartialfloatingengine);
            monteCarlo = partialFloating.NPV();

            diff = Math.Abs(analytical - monteCarlo);

            if (diff > tolerance)
            {
               REPORT_FAILURE_MC("Partial Floating", type, analytical, monteCarlo, tolerance);
            }

            /**
            * Floating
            * **/

            ContinuousFloatingLookbackOption floating = 
               new ContinuousFloatingLookbackOption(minMax,
                                                      floatingPayoff,
                                                      exercise);
            IPricingEngine analyticalFloatingengine =
               new AnalyticContinuousFloatingLookbackEngine(stochProcess);
            floating.setPricingEngine(analyticalFloatingengine);

            analytical = floating.NPV();

            IPricingEngine mcfloatingengine =
               new MakeMCLookbackEngine<ContinuousFloatingLookbackOption.Arguments, ContinuousFloatingLookbackOption.Results,
                                       PseudoRandom, Statistics>
               (stochProcess)
               .withSteps(2000)
               .withAntitheticVariate()
               .withSeed(1)
               .withAbsoluteTolerance(tolerance)
               .value();

            floating.setPricingEngine(mcfloatingengine);
            monteCarlo = floating.NPV();

            diff = Math.Abs(analytical - monteCarlo);

            if (diff > tolerance)
            {
               REPORT_FAILURE_MC("Floating", type, analytical, monteCarlo, tolerance);
            }
         }
      }
   }
}
