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
using System.Linq;

namespace QLCore
{
   //! Black volatility curve modelled as variance curve
   /*! This class calculates time-dependent Black volatilities using
       as input a vector of (ATM) Black volatilities observed in the
       market.

       The calculation is performed interpolating on the variance curve.
       Linear interpolation is used as default; this can be changed
       by the setInterpolation() method.

       For strike dependence, see BlackVarianceSurface.

       \todo check time extrapolation

   */
   public class BlackVarianceCurve : BlackVarianceTermStructure
   {
      DayCounter dayCounter_;
      public override DayCounter dayCounter() { return dayCounter_; }

      Date maxDate_;
      List<double> times_;
      List<double> variances_;
      Interpolation varianceCurve_;

      // required for Handle
      public BlackVarianceCurve(Settings settings, Date referenceDate, List<Date> dates, List<double> blackVolCurve, DayCounter dayCounter,
                                bool forceMonotoneVariance)
         : base(settings, referenceDate, null, BusinessDayConvention.Following, null)
      {

         dayCounter_ = dayCounter;
         maxDate_ = dates.Last();

         Utils.QL_REQUIRE(dates.Count == blackVolCurve.Count, () => "mismatch between date vector and black vol vector");

         // cannot have dates[0]==referenceDate, since the
         // value of the vol at dates[0] would be lost
         // (variance at referenceDate must be zero)
         Utils.QL_REQUIRE(dates[0] > referenceDate, () => "cannot have dates[0] <= referenceDate");

         variances_ = new InitializedList<double>(dates.Count + 1);
         times_ = new InitializedList<double>(dates.Count + 1);
         variances_[0] = 0.0;
         times_[0] = 0.0;
         for (int j = 1; j <= blackVolCurve.Count; j++)
         {
            times_[j] = timeFromReference(dates[j - 1]);

            Utils.QL_REQUIRE(times_[j] > times_[j - 1], () => "dates must be sorted unique!");
            variances_[j] = times_[j] * blackVolCurve[j - 1] * blackVolCurve[j - 1];
            Utils.QL_REQUIRE(variances_[j] >= variances_[j - 1] || !forceMonotoneVariance, () => "variance must be non-decreasing");
         }

         // default: linear interpolation
         setInterpolation<Linear>();
      }

      protected override double blackVarianceImpl(double t, double x)
      {
         if (t <= times_.Last())
         {
            return varianceCurve_.value(t, true);
         }
         else
         {
            // extrapolate with flat vol
            return varianceCurve_.value(times_.Last(), true) * t / times_.Last();
         }
      }

      public void setInterpolation<Interpolator>() where Interpolator : IInterpolationFactory, new ()
      {
         setInterpolation<Interpolator>(FastActivator<Interpolator>.Create());
      }
      public void setInterpolation<Interpolator>(Interpolator i) where Interpolator : IInterpolationFactory, new ()
      {
         varianceCurve_ = i.interpolate(times_, times_.Count, variances_);
         varianceCurve_.update();
      }

      public override Date maxDate() { return maxDate_; }
      public override double minStrike() { return double.MinValue; }
      public override double maxStrike() { return double.MaxValue; }
   }
}
