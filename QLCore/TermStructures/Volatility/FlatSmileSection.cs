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

namespace QLCore
{
   public class FlatSmileSection : SmileSection
   {
      private double vol_;
      private double? atmLevel_;
      public override double? atmLevel()
      {
         return atmLevel_;
      }

      public FlatSmileSection(Date d, double vol, DayCounter dc, Date referenceDate = null, double? atmLevel = null,
                              VolatilityType type = VolatilityType.ShiftedLognormal, double shift = 0.0)
      : base(d, dc, referenceDate, type, shift)
      {
         vol_ = vol;
         atmLevel_ = atmLevel;
      }

      public FlatSmileSection(double exerciseTime, double vol, DayCounter dc, double? atmLevel = null,
                              VolatilityType type = VolatilityType.ShiftedLognormal, double shift = 0.0)
      : base(exerciseTime, dc, type, shift)
      {
         vol_ = vol;
         atmLevel_ = atmLevel;
      }

      public override double minStrike()
      {
         return double.MinValue - shift();
      }

      public override double maxStrike()
      {
         return double.MaxValue;
      }

      protected override double volatilityImpl(double d)
      {
         return vol_;
      }
   }
}
