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
   //! Constant Black volatility, no time-strike dependence
   /*! This class implements the BlackVolatilityTermStructure interface for a constant Black volatility (no time/strike
       dependence). */
   public class BlackConstantVol : BlackVolatilityTermStructure
   {
      private Handle<Quote> volatility_;

      public BlackConstantVol(Date referenceDate, Calendar cal, double volatility, DayCounter dc)
         : base(referenceDate, cal, BusinessDayConvention.Following, dc)
      {
         volatility_ = new Handle<Quote>(new SimpleQuote(volatility));
      }

      public BlackConstantVol(Date referenceDate, Calendar cal, Handle<Quote> volatility, DayCounter dc)
         : base(referenceDate, cal, BusinessDayConvention.Following, dc)
      {
         volatility_ = volatility;

         volatility_.registerWith(update);
      }

      public BlackConstantVol(int settlementDays, Calendar cal, double volatility, DayCounter dc)
         : base(settlementDays, cal, BusinessDayConvention.Following, dc)
      {
         volatility_ = new Handle<Quote>(new SimpleQuote(volatility));
      }

      public BlackConstantVol(int settlementDays, Calendar cal, Handle<Quote> volatility, DayCounter dc)
         : base(settlementDays, cal, BusinessDayConvention.Following, dc)
      {
         volatility_ = volatility;

         volatility_.registerWith(update);
      }


      public override Date maxDate() { return Date.maxDate(); }
      public override double minStrike() { return double.MinValue; }
      public override double maxStrike() { return double.MaxValue; }

      protected override double blackVolImpl(double t, double x) { return volatility_.link.value(); }
   }
}
