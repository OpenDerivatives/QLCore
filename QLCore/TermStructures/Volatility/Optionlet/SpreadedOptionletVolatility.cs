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

namespace QLCore
{
   public class SpreadedOptionletVolatility : OptionletVolatilityStructure
   {
      public SpreadedOptionletVolatility(Handle<OptionletVolatilityStructure> baseVol, Handle<Quote> spread)
      {
         baseVol_ = baseVol;
         spread_ = spread;
         enableExtrapolation(baseVol.link.allowsExtrapolation());
      }
      // All virtual methods of base classes must be forwarded
      // VolatilityTermStructure interface
      public override BusinessDayConvention businessDayConvention() { return baseVol_.link.businessDayConvention(); }
      public override double minStrike() { return baseVol_.link.minStrike(); }
      public override double maxStrike() { return baseVol_.link.maxStrike(); }
      // TermStructure interface
      public override DayCounter dayCounter() { return baseVol_.link.dayCounter(); }
      public override Date maxDate() { return baseVol_.link.maxDate(); }
      public override double maxTime() { return baseVol_.link.maxTime(); }
      public override Date referenceDate() { return baseVol_.link.referenceDate(); }
      public override Calendar calendar() { return baseVol_.link.calendar(); }
      public override int settlementDays() { return baseVol_.link.settlementDays(); }

      // All virtual methods of base classes must be forwarded
      // OptionletVolatilityStructure interface
      protected override SmileSection smileSectionImpl(Date d)
      {
         SmileSection baseSmile = baseVol_.link.smileSection(d, true);
         return new SpreadedSmileSection(baseSmile, spread_);
      }
      protected override SmileSection smileSectionImpl(double optionTime)
      {
         SmileSection baseSmile = baseVol_.link.smileSection(optionTime, true);
         return new SpreadedSmileSection(baseSmile, spread_);
      }
      protected override double volatilityImpl(double t, double s)
      {
         return baseVol_.link.volatility(t, s, true) + spread_.link.value();
      }

      private Handle<OptionletVolatilityStructure> baseVol_;
      private Handle<Quote> spread_;

   }
}
