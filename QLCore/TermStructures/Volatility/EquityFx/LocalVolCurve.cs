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

namespace QLCore
{
   //! Local volatility curve derived from a Black curve
   public class LocalVolCurve : LocalVolTermStructure
   {
      public LocalVolCurve(Handle<BlackVarianceCurve> curve)
         : base(curve.link.businessDayConvention(), curve.link.dayCounter())
      {
         blackVarianceCurve_ = curve;
      }

      // TermStructure interface
      public override Date referenceDate()
      {
         return blackVarianceCurve_.link.referenceDate();
      }

      public override Calendar calendar()
      {
         return blackVarianceCurve_.link.calendar();
      }

      public override DayCounter dayCounter()
      {
         return blackVarianceCurve_.link.dayCounter();
      }

      public override Date maxDate()
      {
         return blackVarianceCurve_.link.maxDate();
      }

      // VolatilityTermStructure interface
      public override double minStrike()
      {
         return double.MinValue;
      }

      public override double maxStrike()
      {
         return double.MaxValue;
      }

      protected override double localVolImpl(double t, double dummy)
      {
         double dt = (1.0 / 365.0);
         double var1 = blackVarianceCurve_.link.blackVariance(t, dummy, true);
         double var2 = blackVarianceCurve_.link.blackVariance(t + dt, dummy, true);
         double derivative = (var2 - var1) / dt;
         return Math.Sqrt(derivative);
      }

      private Handle<BlackVarianceCurve> blackVarianceCurve_;

   }
}
