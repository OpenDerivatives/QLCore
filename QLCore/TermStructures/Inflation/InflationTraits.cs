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
   public class ZeroInflationTraits : ITraits<ZeroInflationTermStructure>
   {
      const double avgInflation = 0.02;
      const double maxInflation = 0.5;

      public Date initialDate(ZeroInflationTermStructure t)
      {
         if (t.indexIsInterpolated())
         {
            return t.referenceDate() - t.observationLag();
         }
         else
         {
            return Utils.inflationPeriod(t.referenceDate() - t.observationLag(),
                                         t.frequency()).Key;
         }
      }

      public double initialValue(ZeroInflationTermStructure t)
      {
         return t.baseRate();
      }

      public double guess<C>(int i, C c, bool validData, int f)
          where C : Curve<ZeroInflationTermStructure>
      {
         if (validData)   // previous iteration value
            return c.data()[i];

         if (i == 1)   // first pillar
            return avgInflation;

         // could/should extrapolate
         return avgInflation;
      }

      public double minValueAfter<C>(int i, C c, bool validData, int f)
          where C : Curve<ZeroInflationTermStructure>
      {
         if (validData)
         {
            double r = c.data().Min();
            return r < 0.0 ? r * 2.0 : r / 2.0;
         }
         return -maxInflation;
      }

      public double maxValueAfter<C>(int i, C c, bool validData, int f)
          where C : Curve<ZeroInflationTermStructure>
      {
         if (validData)
         {
            double r = c.data().Max();
            return r < 0.0 ? r / 2.0 : r * 2.0;
         }
         // no constraints.
         // We choose as max a value very unlikely to be exceeded.
         return maxInflation;
      }

      public void updateGuess(List<double> data, double discount, int i)
      {
         data[i] = discount;
      }

      public int maxIterations()
      {
         return 5;
      }
   }

   public static class ITraitsZeroInflationTermStructure
    {
        public static ZeroInflationTermStructure factory<Interpolator>(this ITraits<ZeroInflationTermStructure> self, Settings settings, 
                                                        Date referenceDate, Calendar calendar, DayCounter dayCounter, Period lag,
                                                        Frequency frequency, bool indexIsInterpolated, double baseZeroRate, 
                                                        Handle<YieldTermStructure> yTS,
                                                        Interpolator interpolator = default(Interpolator))
             where Interpolator : class, IInterpolationFactory, new()
        {
            return new InterpolatedZeroInflationCurve<Interpolator>(settings, referenceDate, calendar, dayCounter, lag, frequency,
                                                                    indexIsInterpolated, baseZeroRate, yTS, interpolator);
        }
    }

   public class YoYInflationTraits : ITraits<YoYInflationTermStructure>
   {
      const double avgInflation = 0.02;
      const double maxInflation = 0.5;

      public Date initialDate(YoYInflationTermStructure t)
      {
         if (t.indexIsInterpolated())
         {
            return t.referenceDate() - t.observationLag();
         }
         else
         {
            return Utils.inflationPeriod(t.referenceDate() - t.observationLag(),
                                         t.frequency()).Key;
         }
      }

      public double initialValue(YoYInflationTermStructure t)
      {
         return t.baseRate();
      }

      public double guess<C>(int i, C c, bool validData, int f)
          where C : Curve<YoYInflationTermStructure>
      {
         if (validData)   // previous iteration value
            return c.data()[i];

         if (i == 1)   // first pillar
            return avgInflation;

         // could/should extrapolate
         return avgInflation;
      }

      public double minValueAfter<C>(int i, C c, bool validData, int f)
          where C : Curve<YoYInflationTermStructure>
      {
         if (validData)
         {
            double r = c.data().Min();
            return r < 0.0 ? r * 2.0 : r / 2.0;
         }
         return -maxInflation;
      }

      public double maxValueAfter<C>(int i, C c, bool validData, int f)
          where C : Curve<YoYInflationTermStructure>
      {
         if (validData)
         {
            double r = c.data().Max();
            return r < 0.0 ? r / 2.0 : r * 2.0;
         }
         // no constraints.
         // We choose as max a value very unlikely to be exceeded.
         return maxInflation;
      }

      public void updateGuess(List<double> data, double discount, int i)
      {
         data[i] = discount;
      }

      public int maxIterations()
      {
         return 40;
      }
   }

    public static class ITraitsYoYInflationTermStructure
    {
        public static YoYInflationTermStructure factory<Interpolator>(this ITraits<YoYInflationTermStructure> self, Settings settings, 
                                                        Date referenceDate, Calendar calendar, DayCounter dayCounter, Period lag,
                                                        Frequency frequency, bool indexIsInterpolated, double baseZeroRate,
                                                        Handle<YieldTermStructure> yTS,
                                                        Interpolator interpolator = default(Interpolator))
             where Interpolator : class, IInterpolationFactory, new()
        {
            return new InterpolatedYoYInflationCurve<Interpolator>(settings, referenceDate, calendar, dayCounter, lag, frequency,
                                                                    indexIsInterpolated, baseZeroRate, yTS, interpolator);
        }
    }
}
