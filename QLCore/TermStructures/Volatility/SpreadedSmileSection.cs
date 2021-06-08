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
   public class SpreadedSmileSection : SmileSection
   {
      public SpreadedSmileSection(SmileSection underlyingSection, Handle<Quote> spread)
      {
         underlyingSection_ = underlyingSection;
         spread_ = spread;
      }
      // SmileSection interface
      public override double minStrike() { return underlyingSection_.minStrike(); }
      public override double maxStrike() { return underlyingSection_.maxStrike(); }
      public override double? atmLevel() { return underlyingSection_.atmLevel(); }
      public override Date exerciseDate() { return underlyingSection_.exerciseDate(); }
      public override double exerciseTime() { return underlyingSection_.exerciseTime(); }
      public override DayCounter dayCounter() { return underlyingSection_.dayCounter(); }
      public override Date referenceDate() { return underlyingSection_.referenceDate(); }
      public override VolatilityType volatilityType() { return underlyingSection_.volatilityType(); }
      public override double shift() { return underlyingSection_.shift(); }
      protected override double volatilityImpl(double k)
      {
         return underlyingSection_.volatility(k) + spread_.link.value();
      }
      private SmileSection underlyingSection_;
      private Handle<Quote> spread_;
   }
}
