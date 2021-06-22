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

namespace QLCore
{
   //! base class for Inter-Bank-Offered-Rate indexes (e.g. %Libor, etc.)
   public class IborIndex : InterestRateIndex
   {
      public IborIndex(string familyName,
                       Period tenor,
                       int settlementDays,
                       Currency currency,
                       Calendar fixingCalendar,
                       BusinessDayConvention convention,
                       bool endOfMonth,
                       DayCounter dayCounter,
                       Settings settings,
                       Handle<YieldTermStructure> h = null)
         : base(familyName, tenor, settlementDays, currency, fixingCalendar, dayCounter, settings)
      {
         convention_ = convention;
         termStructure_ = h ?? new Handle<YieldTermStructure>();
         endOfMonth_ = endOfMonth;
      }

      // InterestRateIndex interface
      public override Date maturityDate(Date valueDate)
      {
         return fixingCalendar().advance(valueDate, tenor_, convention_, endOfMonth_);
      }
      public override double forecastFixing(Date fixingDate)
      {
         Date d1 = valueDate(fixingDate);
         Date d2 = maturityDate(d1);
         double t = dayCounter_.yearFraction(d1, d2);
         Utils.QL_REQUIRE(t > 0.0, () =>
                          "\n cannot calculate forward rate between " +
                          d1 + " and " + d2 +
                          ":\n non positive time (" + t +
                          ") using " + dayCounter_.name() + " daycounter");
         return forecastFixing(d1, d2, t);
      }
      // Inspectors
      public BusinessDayConvention businessDayConvention() { return convention_; }
      public bool endOfMonth() { return endOfMonth_; }
      // the curve used to forecast fixings
      public Handle<YieldTermStructure> forwardingTermStructure() { return termStructure_; }
      // Other methods
      // returns a copy of itself linked to a different forwarding curve
      public virtual IborIndex clone(Handle<YieldTermStructure> forwarding)
      {
         IborIndex tmp = new IborIndex(familyName(), tenor(), fixingDays(), currency(), fixingCalendar(),
                                       businessDayConvention(), endOfMonth(), dayCounter(), settings(), forwarding);
         tmp.data_ = data_;
         return tmp;
      }

      protected BusinessDayConvention convention_;
      protected Handle<YieldTermStructure> termStructure_;
      protected bool endOfMonth_;


      public double forecastFixing(Date d1, Date d2, double t)
      {
         Utils.QL_REQUIRE(!termStructure_.empty(), () => "null term structure set to this instance of " + name());
         double disc1 = termStructure_.link.discount(d1);
         double disc2 = termStructure_.link.discount(d2);
         return (disc1 / disc2 - 1.0) / t;
      }


      // need by CashFlowVectors
      public IborIndex() { }

      //override some function to update term structure
      public override void addFixings(List<Date> d, List<double> v, bool forceOverwrite = false)
      {
         base.addFixings(d, v, forceOverwrite);
         
         if (!termStructure_.empty())
            termStructure_.link.update();
      }

      public override void addFixing(Date d, double v, bool forceOverwrite = false)
      {
         base.addFixing(d, v, forceOverwrite);

         if (!termStructure_.empty())
            termStructure_.link.update();
      }

   }

   public class OvernightIndex : IborIndex
   {
      public OvernightIndex(string familyName,
                            int settlementDays,
                            Currency currency,
                            Calendar fixingCalendar,
                            DayCounter dayCounter,
                            Settings settings,
                            Handle<YieldTermStructure> h = null) :

         base(familyName, new Period(1, TimeUnit.Days), settlementDays,
              currency, fixingCalendar, BusinessDayConvention.Following, false, dayCounter, settings, h)
      {}

      //! returns a copy of itself linked to a different forwarding curve
      public new OvernightIndex clone(Handle<YieldTermStructure> h)
      {
         OvernightIndex tmp = new OvernightIndex(familyName(), fixingDays(), currency(), fixingCalendar(),
                                   dayCounter(), settings(), h);
         tmp.data_ = data_;
         return tmp;
      }
   }
}
