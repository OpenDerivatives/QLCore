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
   //! Flat interest-rate curve
   public class FlatForward : YieldTermStructure
   {
      private Quote forward_;
      private Compounding compounding_;
      private Frequency frequency_;
      private InterestRate rate_;

      // constructors
      public FlatForward(Date referenceDate, Quote forward, DayCounter dayCounter) :
         this(referenceDate, forward, dayCounter, Compounding.Continuous, Frequency.Annual) {}
      public FlatForward(Date referenceDate, Quote forward, DayCounter dayCounter, Compounding compounding) :
         this(referenceDate, forward, dayCounter, compounding, Frequency.Annual) {}
      public FlatForward(Date referenceDate, Quote forward, DayCounter dayCounter, Compounding compounding, Frequency frequency) :
         base(referenceDate, new Calendar(), dayCounter)
      {
         forward_ = forward;
         compounding_ = compounding;
         frequency_ = frequency;
      }

      public FlatForward(Date referenceDate, double forward, DayCounter dayCounter) :
         this(referenceDate, forward, dayCounter, Compounding.Continuous, Frequency.Annual) {}
      public FlatForward(Date referenceDate, double forward, DayCounter dayCounter, Compounding compounding) :
         this(referenceDate, forward, dayCounter, compounding, Frequency.Annual) {}
      public FlatForward(Date referenceDate, double forward, DayCounter dayCounter, Compounding compounding, Frequency frequency) :
         base(referenceDate, new Calendar(), dayCounter)
      {
         forward_ = new SimpleQuote(forward);
         compounding_ = compounding;
         frequency_ = frequency;
      }

      public FlatForward(int settlementDays, Calendar calendar, Quote forward, DayCounter dayCounter) :
         this(settlementDays, calendar, forward, dayCounter, Compounding.Continuous, Frequency.Annual) {}
      public FlatForward(int settlementDays, Calendar calendar, Quote forward, DayCounter dayCounter, Compounding compounding) :
         this(settlementDays, calendar, forward, dayCounter, compounding, Frequency.Annual) {}
      public FlatForward(int settlementDays, Calendar calendar, Quote forward, DayCounter dayCounter, Compounding compounding, Frequency frequency) :
         base(settlementDays, calendar, dayCounter)
      {
         forward_ = forward;
         compounding_ = compounding;
         frequency_ = frequency;
      }

      public FlatForward(int settlementDays, Calendar calendar, double forward, DayCounter dayCounter) :
         this(settlementDays, calendar, forward, dayCounter, Compounding.Continuous, Frequency.Annual) {}
      public FlatForward(int settlementDays, Calendar calendar, double forward, DayCounter dayCounter, Compounding compounding) :
         this(settlementDays, calendar, forward, dayCounter, compounding, Frequency.Annual) {}
      public FlatForward(int settlementDays, Calendar calendar, double forward, DayCounter dayCounter,
                         Compounding compounding, Frequency frequency) :
         base(settlementDays, calendar, dayCounter)
      {
         forward_ = new SimpleQuote(forward);
         compounding_ = compounding;
         frequency_ = frequency;
      }

      // TermStructure interface
      public override Date maxDate() { return Date.maxDate(); }

      protected internal override double discountImpl(double t)
      {
         calculate();
         return rate_.discountFactor(t);
      }

      protected override void performCalculations()
      {
         rate_ = new InterestRate(forward_.value(), dayCounter(), compounding_, frequency_);
      }
   }
}
