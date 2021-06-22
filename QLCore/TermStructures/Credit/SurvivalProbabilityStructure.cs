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
using System.Text;
using System.Threading.Tasks;

namespace QLCore
{
   /// <summary>
   /// Hazard-rate term structure
   /// This abstract class acts as an adapter to
   /// DefaultProbabilityTermStructure allowing the programmer to implement
   /// only the survivalProbabilityImpl(Time) method in derived classes.
   /// <remarks>
   /// Hazard rates and default densities are calculated from survival probabilities.
   /// </remarks>
   /// </summary>
   public abstract class SurvivalProbabilityStructure : DefaultProbabilityTermStructure
   {
      public SurvivalProbabilityStructure(Settings settings, 
                                          DayCounter dayCounter = null,
                                          List<Handle<Quote>> jumps = null,
                                          List<Date> jumpDates = null)
         : base(settings, dayCounter, jumps, jumpDates) {}
      public SurvivalProbabilityStructure(Settings settings, 
                                          Date referenceDate,
                                          Calendar cal = null,
                                          DayCounter dayCounter = null,
                                          List<Handle<Quote>> jumps = null,
                                          List<Date> jumpDates = null)
         : base(settings, referenceDate, cal, dayCounter, jumps, jumpDates) { }

      public SurvivalProbabilityStructure(Settings settings, 
                                          int settlementDays,
                                          Calendar cal,
                                          DayCounter dayCounter = null,
                                          List<Handle<Quote>> jumps = null,
                                          List<Date> jumpDates = null)
         : base(settings, settlementDays, cal, dayCounter, jumps, jumpDates) { }

      /// <summary>
      /// DefaultProbabilityTermStructure implementation
      /// </summary>
      /// <remarks>
      /// This implementation uses numerical differentiation,which might be inefficient and inaccurate.
      /// Derived classes should override it if a more efficient implementation is available.
      /// </remarks>
      /// <param name="t"></param>
      /// <returns></returns>
      protected internal override double defaultDensityImpl(double t)
      {
         double dt = 0.0001;
         double t1 = Math.Max(t - dt, 0.0);
         double t2 = t + dt;

         double p1 = survivalProbabilityImpl(t1);
         double p2 = survivalProbabilityImpl(t2);

         return (p1 - p2) / (t2 - t1);
      }
   }
}
