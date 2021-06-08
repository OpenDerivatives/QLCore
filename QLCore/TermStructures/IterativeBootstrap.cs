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

namespace QLCore
{
   public interface IBootStrap<T>
   {
      void setup(T ts);
      void calculate();
   }

   public class IterativeBootstrapForYield : IterativeBootstrap<PiecewiseYieldCurve, YieldTermStructure>
   {
   }

   public class IterativeBootstrapForInflation : IterativeBootstrap<PiecewiseZeroInflationCurve, ZeroInflationTermStructure>
   {
   }

   public class IterativeBootstrapForYoYInflation : IterativeBootstrap<PiecewiseYoYInflationCurve, YoYInflationTermStructure>
   {
   }

   public class IterativeBootstrapForCds : IterativeBootstrap<PiecewiseDefaultCurve, DefaultProbabilityTermStructure>
   {
   }


   //! Universal piecewise-term-structure boostrapper.
   public class IterativeBootstrap<T, U>: IBootStrap<T>
      where T : Curve<U>, new()
      where U : TermStructure
   {
      private double? accuracy_;
      private double? minValue_, maxValue_;
      private int maxAttempts_;
      private double maxFactor_;
      private double minFactor_;
      private bool dontThrow_;
      private int dontThrowSteps_;
      private bool validCurve_;
      private T ts_;
      private int n_;
      private Brent firstSolver_ = new Brent();
      private FiniteDifferenceNewtonSafe solver_ = new FiniteDifferenceNewtonSafe();
      private bool initialized_, loopRequired_;
      private int firstAliveHelper_, alive_;
      private List<double> previousData_;
      private List<BootstrapError<T, U>> errors_;

      public IterativeBootstrap(double? accuracy = null,
                                double? minValue = null,
                                double? maxValue = null,
                                int maxAttempts = 1,
                                double maxFactor = 2.0,
                                double minFactor = 2.0,
                                bool dontThrow = false,
                                int dontThrowSteps = 10)
      {
         ts_ = FastActivator<T>.Create();
         initialized_ = false;
         validCurve_ = false;
         accuracy_ = accuracy;
         minValue_ = minValue;
         maxValue_ = maxValue;
         maxAttempts_ = maxAttempts;
         maxFactor_ = maxFactor;
         minFactor_ = minFactor;
         dontThrow_ = dontThrow;
         dontThrowSteps_ = dontThrowSteps;
      }

      private void initialize()
      {
         // ensure helpers are sorted
         ts_.instruments_.Sort((x, y) => x.pillarDate().CompareTo(y.pillarDate()));

         // skip expired helpers
         Date firstDate = ts_.initialDate();
         Utils.QL_REQUIRE(ts_.instruments_[n_ - 1].pillarDate() > firstDate, () => "all instruments expired");
         firstAliveHelper_ = 0;
         while (ts_.instruments_[firstAliveHelper_].pillarDate() <= firstDate)
            ++firstAliveHelper_;
         alive_ = n_ - firstAliveHelper_;
         Utils.QL_REQUIRE(alive_ >= ts_.interpolator_.requiredPoints - 1, () =>
                          "not enough alive instruments: " + alive_ +
                          " provided, " + (ts_.interpolator_.requiredPoints - 1) +
                          " required");


         if (ts_.dates_ == null)
         {
            ts_.dates_ = new InitializedList<Date>(alive_ + 1);
            ts_.times_ = new InitializedList<double>(alive_ + 1);
         }
         else if (ts_.dates_.Count != alive_ + 1)
         {
            ts_.dates_.Resize(alive_ + 1);
            ts_.times_.Resize(alive_ + 1);
         }

         List<Date> dates = ts_.dates_;
         List<double> times = ts_.times_;

         errors_ = new List<BootstrapError<T, U>>(alive_ + 1);
         dates[0] = firstDate ;
         times[0] = (ts_.timeFromReference(dates[0]));
         Date latestRelevantDate, maxDate = firstDate;
         for (int i = 1, j = firstAliveHelper_; j < n_; ++i, ++j)
         {
            BootstrapHelper<U> helper = ts_.instruments_[j];
            dates[i] = (helper.pillarDate());
            times[i] = (ts_.timeFromReference(dates[i]));
            // check for duplicated maturity
            Utils.QL_REQUIRE(dates[i - 1] != dates[i], () => "more than one instrument with maturity " + dates[i]);
            latestRelevantDate = helper.latestRelevantDate();
            // check that the helper is really extending the curve, i.e. that
            // pillar-sorted helpers are also sorted by latestRelevantDate
            Utils.QL_REQUIRE(latestRelevantDate > maxDate, () =>
                             (j + 1) + " instrument (pillar: " +
                             dates[i] + ") has latestRelevantDate (" +
                             latestRelevantDate + ") before or equal to " +
                             "previous instrument's latestRelevantDate (" +
                             maxDate + ")");
            maxDate = latestRelevantDate;

            // when a pillar date is different from the last relevant date the
            // convergence loop is required even if the Interpolator is local
            if (dates[i] != latestRelevantDate)
               loopRequired_ = true;

            errors_.Add(new BootstrapError<T, U>(ts_, helper, i));
         }
         ts_.maxDate_ = maxDate;

         // set initial guess only if the current curve cannot be used as guess
         if (!validCurve_ || ts_.data_.Count != alive_ + 1)
         {
            // ts_->data_[0] is the only relevant item,
            // but reasonable numbers might be needed for the whole data vector
            // because, e.g., of interpolation's early checks
            ts_.data_ = new InitializedList<double>(alive_ + 1, ts_.initialValue());
            previousData_ = new List<double>(alive_ + 1);
         }
         initialized_ = true;
      }

      public void setup(T ts)
      {
         ts_ = ts;

         n_ = ts_.instruments_.Count;
         Utils.QL_REQUIRE(n_ > 0, () => "no bootstrap helpers given");

         Utils.QL_REQUIRE(n_ + 1 >= ts_.interpolator_.requiredPoints, () =>
                          "not enough instruments: " + n_ + " provided, " + (ts_.interpolator_.requiredPoints - 1) + " required");

         loopRequired_ = ts_.interpolator_.global;
      }

      public void calculate()
      {
         // we might have to call initialize even if the curve is initialized
         // and not moving, just because helpers might be date relative and change
         // with evaluation date change.
         // anyway it makes little sense to use date relative helpers with a
         // non-moving curve if the evaluation date changes
         if (!initialized_ || ts_.moving_)
            initialize();

         // setup helpers
         for (int j = firstAliveHelper_; j < n_; ++j)
         {
            BootstrapHelper<U> helper = ts_.instruments_[j];
            // check for valid quote
            Utils.QL_REQUIRE(helper.quote().link.isValid(), () =>
                             (j + 1) + " instrument (maturity: " +
                             helper.pillarDate() + ") has an invalid quote");
            // don't try this at home!
            // This call creates helpers, and removes "const".
            // There is a significant interaction with observability.
            ts_.setTermStructure(ts_.instruments_[j]);
         }

         List<double> times = ts_.times_;
         List<double> data = ts_.data_;
         double accuracy = accuracy_ != null ? accuracy_.Value : ts_.accuracy_;
         int maxIterations = ts_.maxIterations() - 1;

         // there might be a valid curve state to use as guess
         bool validData = validCurve_;

         for (int iteration = 0; ; ++iteration)
         {
            previousData_ = new List<double>(ts_.data_);

            List<double?> minValues = new InitializedList<double?>(alive_ + 1, null);
            List<double?> maxValues = new InitializedList<double?>(alive_ + 1, null);
            List<int> attempts = new InitializedList<int>(alive_ + 1, 1);

            for (int i = 1; i <= alive_; ++i)
            {
               // shorter aliases for readability and to avoid duplication
               double? min = minValues[i];
               double? max = maxValues[i];

               // bracket root and calculate guess
               if (min == null)
               {
                  min = (minValue_ != null ? minValue_ :
                           ts_.minValueAfter(i, ts_, validData, firstAliveHelper_));
                  max = (maxValue_ != null ? maxValue_ :
                           ts_.maxValueAfter(i, ts_, validData, firstAliveHelper_));
               }
               else
               {
                  min = (min.Value < 0.0 ? min.Value * minFactor_ : min.Value / minFactor_);
                  max = (max.Value > 0.0 ? max.Value * maxFactor_ : max.Value / maxFactor_);
               }
               double guess = ts_.guess(i, ts_, validData, firstAliveHelper_);

               // adjust guess if needed
               if (guess >= max.Value)
                  guess = max.Value - (max.Value - min.Value) / 5.0;
               else if (guess <= min.Value)
                  guess = min.Value + (max.Value - min.Value) / 5.0;

               // extend interpolation if needed
               if (!validData)
               {
                  try
                  {
                     // extend interpolation a point at a time
                     // including the pillar to be boostrapped
                     ts_.interpolation_ = ts_.interpolator_.interpolate(ts_.times_, i + 1, ts_.data_);
                  }
                  catch (Exception)
                  {
                     if (!ts_.interpolator_.global)
                        throw; // no chance to fix it in a later iteration

                     // otherwise use Linear while the target
                     // interpolation is not usable yet
                     ts_.interpolation_ = new Linear().interpolate(ts_.times_, i + 1, ts_.data_);
                  }
                  ts_.interpolation_.update();
               }

               try
               {
                  var error = new BootstrapError<T, U>(ts_, ts_.instruments_[i - 1], i);
                  if (validData)
                     ts_.data_[i] = solver_.solve(error, accuracy, guess, min.Value, max.Value);
                  else
                     ts_.data_[i] = firstSolver_.solve(error, accuracy, guess, min.Value, max.Value);
               }
               catch (Exception e)
               {
                  if (validCurve_)
                  {
                      // the previous curve state might have been a
                     // bad guess, so we retry without using it.
                     // This would be tricky to do here (we're
                     // inside multiple nested for loops, we need
                     // to re-initialize...), so we invalidate the
                     // curve, make a recursive call and then exit.
                     validCurve_ = initialized_ = false;
                     calculate();
                     return;
                  }

                  // If we have more attempts left on this iteration, try again. Note that the max and min
                  // bounds will be widened on the retry.
                  if (attempts[i] < maxAttempts_) {
                     attempts[i]++;
                     i--;
                     continue;
                  }

                  if (dontThrow_) 
                  {
                     // Use the fallback value
                     var error = new BootstrapError<T, U>(ts_, ts_.instruments_[i - 1], i);
                     ts_.data_[i] = dontThrowFallback(error, min.Value, max.Value, dontThrowSteps_);

                     // Remember to update the interpolation. If we don't and we are on the last "i", we will still
                     // have the last attempted value in the solver being used in ts_->interpolation_.
                     ts_.interpolation_.update();
                  } 
                  else 
                  {
                     Utils.QL_FAIL((iteration + 1) + " iteration: failed " +
                                "at " + (i) + " alive instrument, " +
                                "pillar " + ts_.instruments_[i - 1].pillarDate() + ", " +
                                "maturity " + ts_.instruments_[i - 1].maturityDate() +
                                ", reference date " + ts_.dates_[0] +
                                ": " + e.Message);
                  }
               }
            }

            if (!loopRequired_)
               break;     // no need for convergence loop

            // exit condition
            double change = Math.Abs(data[1] - previousData_[1]);
            for (int i = 2; i <= alive_; ++i)
               change = Math.Max(change, Math.Abs(data[i] - previousData_[i]));
            if (change <= accuracy)    // convergence reached
               break;

            Utils.QL_REQUIRE(iteration < maxIterations, () =>
                             "convergence not reached after " + iteration +
                             " iterations; last improvement " + change +
                             ", required accuracy " + accuracy);
            validData = true;
         }
         validCurve_ = true;
      }

      public double dontThrowFallback(BootstrapError<T, U> error,
                                      double xMin, double xMax, 
                                      int steps) 
      {
         Utils.QL_REQUIRE(xMin < xMax, () => "Expected xMin to be less than xMax");

         // Set the initial value of the result to xMin and store the absolute bootstrap error at xMin
         double result = xMin;
         double absError = Math.Abs(error.value(xMin));
         double minError = absError;

         // Step out to xMax
         double stepSize = (xMax - xMin) / steps;
         for (int i = 0; i < steps; i++) {

            // Get absolute bootstrap error at updated x value
            xMin += stepSize;
            absError = Math.Abs(error.value(xMin));

            // If this absolute bootstrap error is less than the minimum, update result and minError
            if (absError < minError) {
                  result = xMin;
                  minError = absError;
            }
         }

         return result;
      }
   }
}
