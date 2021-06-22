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
   //! Constant local volatility, no time-strike dependence
   /*! This class implements the LocalVolatilityTermStructure
       interface for a constant local volatility (no time/asset
       dependence).  Local volatility and Black volatility are the
       same when volatility is at most time dependent, so this class
       is basically a proxy for BlackVolatilityTermStructure.
   */
   public class LocalConstantVol : LocalVolTermStructure
   {
      Handle<Quote> volatility_;
      DayCounter dayCounter_;

      public LocalConstantVol(Settings settings, Date referenceDate, double volatility, DayCounter dc)
         : base(settings, referenceDate, null, BusinessDayConvention.Following, null)
      {
         volatility_ = new Handle<Quote>(new SimpleQuote(volatility));
         dayCounter_ = dc;
      }

      public LocalConstantVol(Settings settings, Date referenceDate, Handle<Quote> volatility, DayCounter dc)
         : base(settings, referenceDate, null, BusinessDayConvention.Following, null)
      {
         volatility_ = volatility;
         dayCounter_ = dc;
      }

      public LocalConstantVol(Settings settings, int settlementDays, Calendar calendar, double volatility, DayCounter dayCounter)
         : base(settings, settlementDays, calendar, BusinessDayConvention.Following, null)
      {
         volatility_ = new Handle<Quote>(new SimpleQuote(volatility));
         dayCounter_ = dayCounter;
      }

      public LocalConstantVol(Settings settings, int settlementDays, Calendar calendar, Handle<Quote> volatility, DayCounter dayCounter)
         : base(settings, settlementDays, calendar, BusinessDayConvention.Following, null)
      {
         volatility_ = volatility;
         dayCounter_ = dayCounter;
      }

      // TermStructure interface
      public override DayCounter dayCounter() { return dayCounter_; }
      public override Date maxDate() { return Date.maxDate(); }
      // VolatilityTermStructure interface
      public override double minStrike() { return double.MinValue; }
      public override double maxStrike() { return double.MaxValue; }

      protected override double localVolImpl(double t, double s) { return volatility_.link.value(); }
   }
}
