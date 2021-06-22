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
   //! Constant callable-bond volatility, no time-strike dependence
   public class CallableBondConstantVolatility : CallableBondVolatilityStructure
   {
      public CallableBondConstantVolatility(Settings settings, Date referenceDate, double volatility, DayCounter dayCounter)
         : base(settings, referenceDate, null, null, BusinessDayConvention.Following)
      {
         volatility_ = new Handle<Quote>(new SimpleQuote(volatility));
         dayCounter_ = dayCounter;
         maxBondTenor_ = new Period(100, TimeUnit.Years);
      }

      public CallableBondConstantVolatility(Settings settings, Date referenceDate, Handle<Quote> volatility, DayCounter dayCounter)
         : base(settings, referenceDate, null, null, BusinessDayConvention.Following)
      {
         volatility_ = volatility;
         dayCounter_ = dayCounter;
         maxBondTenor_ = new Period(100, TimeUnit.Years);
      }

      public CallableBondConstantVolatility(Settings settings, int settlementDays, Calendar calendar, double volatility, DayCounter dayCounter)
         : base(settings, settlementDays, calendar, null, BusinessDayConvention.Following)
      {
         volatility_ = new Handle<Quote>(new SimpleQuote(volatility));
         dayCounter_ = dayCounter;
         maxBondTenor_ = new Period(100, TimeUnit.Years);
      }

      public CallableBondConstantVolatility(Settings settings, int settlementDays, Calendar calendar, Handle<Quote> volatility, DayCounter dayCounter)
         : base(settings, settlementDays, calendar, null, BusinessDayConvention.Following)
      {
         volatility_ = volatility;
         dayCounter_ = dayCounter;
         maxBondTenor_ = new Period(100, TimeUnit.Years);
      }

      // TermStructure interface
      public override DayCounter dayCounter() { return dayCounter_; }
      public override Date maxDate() { return Date.maxDate(); }

      // CallableBondConstantVolatility interface
      public override Period maxBondTenor() {return maxBondTenor_;}
      public override double maxBondLength() {return double.MaxValue;}
      public override double minStrike() {return double.MinValue;}
      public override double maxStrike()  {return double.MaxValue;}

      protected override double volatilityImpl(double d1, double d2, double d3)
      {
         return volatility_.link.value();
      }
      protected override SmileSection smileSectionImpl(double optionTime, double bondLength)
      {
         double atmVol = volatility_.link.value();
         return new FlatSmileSection(this.settings(), optionTime, atmVol, dayCounter_);
      }
      protected override double volatilityImpl(Date d, Period p, double d1)
      {
         return volatility_.link.value();
      }

      private Handle<Quote> volatility_;
      private DayCounter dayCounter_;
      private Period maxBondTenor_;
   }
}
