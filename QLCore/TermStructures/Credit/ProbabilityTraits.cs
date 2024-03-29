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
using System.Collections.Generic;
using System.Linq;

namespace QLCore
{
   public static class ITraitsDefaultTermStructure
    {
        public static DefaultProbabilityTermStructure factory<Interpolator>(this ITraits<DefaultProbabilityTermStructure> self,
                                                               Settings settings,
                                                               DayCounter dayCounter,
                                                               List<Handle<Quote>> jumps = null,
                                                               List<Date> jumpDates = null,
                                                               Interpolator interpolator = default(Interpolator))
            where Interpolator : class, IInterpolationFactory, new()
        {
            if (self.GetType().Equals(typeof(SurvivalProbability)))
                return new InterpolatedSurvivalProbabilityCurve<Interpolator>(settings, dayCounter, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(HazardRate)))
                return new InterpolatedHazardRateCurve<Interpolator>(settings, dayCounter, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(DefaultDensity)))
                return new InterpolatedDefaultDensityCurve<Interpolator>(settings, dayCounter, jumps, jumpDates, interpolator);
            else
                return null;
        }

        public static DefaultProbabilityTermStructure factory<Interpolator>(this ITraits<DefaultProbabilityTermStructure> self,
                                                               Settings settings,
                                                               Date referenceDate,
                                                               DayCounter dayCounter,
                                                               List<Handle<Quote>> jumps = null,
                                                               List<Date> jumpDates = null,
                                                               Interpolator interpolator = default(Interpolator))
            where Interpolator : class, IInterpolationFactory, new()
        {
            if (self.GetType().Equals(typeof(SurvivalProbability)))
                return new InterpolatedSurvivalProbabilityCurve<Interpolator>(settings, referenceDate, dayCounter, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(HazardRate)))
                return new InterpolatedHazardRateCurve<Interpolator>(settings, referenceDate, dayCounter, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(DefaultDensity)))
                return new InterpolatedDefaultDensityCurve<Interpolator>(settings, referenceDate, dayCounter, jumps, jumpDates, interpolator);
            else
                return null;
        }

        public static DefaultProbabilityTermStructure factory<Interpolator>(this ITraits<DefaultProbabilityTermStructure> self,
                                                               Settings settings,
                                                               int settlementDays,
                                                               Calendar calendar,
                                                               DayCounter dayCounter,
                                                               List<Handle<Quote>> jumps = null,
                                                               List<Date> jumpDates = null,
                                                               Interpolator interpolator = default(Interpolator))
            where Interpolator : class, IInterpolationFactory, new()
        {
            if (self.GetType().Equals(typeof(SurvivalProbability)))
                return new InterpolatedSurvivalProbabilityCurve<Interpolator>(settings, settlementDays, calendar, dayCounter, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(HazardRate)))
                return new InterpolatedHazardRateCurve<Interpolator>(settings, settlementDays, calendar, dayCounter, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(DefaultDensity)))
                return new InterpolatedDefaultDensityCurve<Interpolator>(settings, settlementDays, calendar, dayCounter, jumps, jumpDates, interpolator);
            else
                return null;
        }

        public static DefaultProbabilityTermStructure factory<Interpolator>(this ITraits<DefaultProbabilityTermStructure> self,
                                                               Settings settings,
                                                               List<Date> dates,
                                                               List<double> densities,
                                                               DayCounter dayCounter,
                                                               Calendar calendar = null,
                                                               List<Handle<Quote>> jumps = null,
                                                               List<Date> jumpDates = null,
                                                               Interpolator interpolator = default(Interpolator))
            where Interpolator : class, IInterpolationFactory, new()
        {
            if (self.GetType().Equals(typeof(SurvivalProbability)))
                return new InterpolatedSurvivalProbabilityCurve<Interpolator>(settings, dates, densities, dayCounter, calendar, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(HazardRate)))
                return new InterpolatedHazardRateCurve<Interpolator>(settings, dates, densities, dayCounter, calendar, jumps, jumpDates, interpolator);
            else if (self.GetType().Equals(typeof(DefaultDensity)))
                return new InterpolatedDefaultDensityCurve<Interpolator>(settings, dates, densities, dayCounter, calendar, jumps, jumpDates, interpolator);
            else
                return null;
        }

        public static DefaultProbabilityTermStructure factory<Interpolator>(this ITraits<DefaultProbabilityTermStructure> self,
                                                               Settings settings,
                                                               List<Date> dates,
                                                               List<double> densities,
                                                               DayCounter dayCounter,
                                                               Calendar calendar,
                                                               Interpolator interpolator)
            where Interpolator : class, IInterpolationFactory, new()
        {
            if (self.GetType().Equals(typeof(HazardRate)))
                return new InterpolatedHazardRateCurve<Interpolator>(settings, dates, densities, dayCounter, calendar, interpolator);
            else if (self.GetType().Equals(typeof(DefaultDensity)))
                return new InterpolatedDefaultDensityCurve<Interpolator>(settings, dates, densities, dayCounter, calendar, interpolator);
            else
                return null;
        }

        public static DefaultProbabilityTermStructure factory<Interpolator>(this ITraits<DefaultProbabilityTermStructure> self,
                                                               Settings settings,
                                                               List<Date> dates,
                                                               List<double> densities,
                                                               DayCounter dayCounter,
                                                               Interpolator interpolator)
            where Interpolator : class, IInterpolationFactory, new()
        {
            if (self.GetType().Equals(typeof(HazardRate)))
                return new InterpolatedHazardRateCurve<Interpolator>(settings, dates, densities, dayCounter, interpolator);
            else if (self.GetType().Equals(typeof(DefaultDensity)))
                return new InterpolatedDefaultDensityCurve<Interpolator>(settings, dates, densities, dayCounter, interpolator);
            else
                return null;
        }
    }

   /// <summary>
   /// Survival-Probability-curve traits
   /// </summary>
	public class SurvivalProbability : ITraits<DefaultProbabilityTermStructure>
   {
      const double avgHazardRate = 0.01;
      const double maxHazardRate = 1.0;

      public Date initialDate(DefaultProbabilityTermStructure c) { return c.referenceDate(); }   // start of curve data
      public double initialValue(DefaultProbabilityTermStructure c) { return 1; }    // value at reference date
      public void updateGuess(List<double> data, double discount, int i) { data[i] = discount; }
      public int maxIterations() { return 50; }   // upper bound for convergence loop

      public double guess<C>(int i, C c, bool validData, int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData) // previous iteration value
            return c.data()[i];

         if (i == 1) // first pillar
            return 1.0 / (1.0 + avgHazardRate * 0.25);

         // extrapolate
         Date d = c.dates()[i];
         return (c as DefaultProbabilityTermStructure).survivalProbability(d, true);
      }

      public double minValueAfter<C>(int i, C c, bool validData, int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData)
         {
            return c.data().Last() / 2.0;
         }
         double dt = c.times()[i] - c.times()[i - 1];
         return c.data()[i - 1] * Math.Exp(-maxHazardRate * dt);
      }

      public double maxValueAfter<C>(int i, C c, bool validData, int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         // survival probability cannot increase
         return c.data()[i - 1];
      }
   }

   /// <summary>
   ///  Hazard-rate-curve traits 
   /// </summary>
   public class HazardRate  : ITraits<DefaultProbabilityTermStructure>
   {
      const double avgHazardRate = 0.01;
      const double maxHazardRate = 1.0;

      public Date initialDate(DefaultProbabilityTermStructure c)
      {
         return c.referenceDate();
      }
      public double initialValue(DefaultProbabilityTermStructure c)
      {
         return avgHazardRate;
      }
      public double guess<C>(int i, C c,bool validData,int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData) // previous iteration value
            return c.data()[i];

         if (i == 1) // first pillar
            return avgHazardRate;

         // extrapolate
         Date d = c.dates()[i];
         return (c as DefaultProbabilityTermStructure).hazardRate(d, true);
      }
      public double minValueAfter<C>(int i, C c, bool validData, int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData)
         {
            double r = c.data().Min();
            return r / 2.0;
         }
         return Const.QL_EPSILON;
      }
      public double maxValueAfter<C>(int i, C c, bool validData, int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData)
         {
            double r = c.data().Max();
            return r * 2.0;
         }
         // no constraints.
         // We choose as max a value very unlikely to be exceeded.
         return maxHazardRate;
      }
      public  void updateGuess(List<double> data,double rate,int i)
      {
         data[i] = rate;
         if (i == 1)
            data[0] = rate; // first point is updated as well
      }
      public int maxIterations() { return 30; }
   }

   /// <summary>
   /// Default-density-curve traits
   /// </summary>
   public class DefaultDensity : ITraits<DefaultProbabilityTermStructure>
   {
      const double avgHazardRate = 0.01;
      const double maxHazardRate = 1.0;

      public Date initialDate( DefaultProbabilityTermStructure c)
      {
         return c.referenceDate();
      }
      public double initialValue(DefaultProbabilityTermStructure c)
      {
         return avgHazardRate;
      }
      public double guess<C>(int i,C c,bool validData,int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData) // previous iteration value
            return c.data()[i];

         if (i == 1) // first pillar
            return avgHazardRate;

         // extrapolate
         Date d = c.dates()[i];
         return (c as DefaultProbabilityTermStructure).defaultDensity(d, true);
      }
      public double minValueAfter<C>(int i,C c,bool validData,int f)  where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData)
         {
            double r = c.data().Min();
            return r / 2.0;
         }
         return Const.QL_EPSILON;
      }
      public double maxValueAfter<C>(int i, C c, bool validData, int f) where C : Curve<DefaultProbabilityTermStructure>
      {
         if (validData)
         {
            double r = c.data().Max();
            return r * 2.0;
         }
         // no constraints.
         // We choose as max a value very unlikely to be exceeded.
         return maxHazardRate;
      }
      public void updateGuess(List<double> data,double density,int i)
      {
         data[i] = density;
         if (i == 1)
            data[0] = density; // first point is updated as well
      }
      public int maxIterations() { return 30; }
   }
}
