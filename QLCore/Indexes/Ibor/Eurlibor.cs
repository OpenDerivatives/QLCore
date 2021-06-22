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
   public static partial class Utils
   {
      public static BusinessDayConvention eurliborConvention(Period p)
      {
         switch (p.units())
         {
            case TimeUnit.Days:
            case TimeUnit.Weeks:
               return BusinessDayConvention.Following;
            case TimeUnit.Months:
            case TimeUnit.Years:
               return BusinessDayConvention.ModifiedFollowing;
            default:
               throw new ArgumentException("Unknown TimeUnit: " + p.units());
         }
      }

      public static bool eurliborEOM(Period p)
      {
         switch (p.units())
         {
            case TimeUnit.Days:
            case TimeUnit.Weeks:
               return false;
            case TimeUnit.Months:
            case TimeUnit.Years:
               return true;
            default:
               throw new ArgumentException("Unknown TimeUnit: " + p.units());
         }
      }
   }

   //! base class for all ICE %EUR %LIBOR indexes but the O/N
   /*! Euro LIBOR fixed by ICE.

       See <https://www.theice.com/marketdata/reports/170>.

       \warning This is the rate fixed in London by BBA. Use Euribor if
                you're interested in the fixing by the ECB.
   */
   public class EURLibor : IborIndex
   {
      private Calendar target_;

      // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
      // JoinBusinessDays is the fixing calendar for
      // all indexes but o/n

      public EURLibor(Period tenor, Settings settings)
         : base("EURLibor", tenor, 2, new EURCurrency(), new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new TARGET(),
                                                                           JointCalendar.JointCalendarRule.JoinHolidays),
                Utils.eurliborConvention(tenor), Utils.eurliborEOM(tenor), new Actual360(), settings,
                new Handle<YieldTermStructure>())
      {
         target_ = new TARGET();
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
      }

      public EURLibor(Period tenor, Settings settings, Handle<YieldTermStructure> h)
         : base("EURLibor", tenor, 2, new EURCurrency(), new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new TARGET(),
                                                                           JointCalendar.JointCalendarRule.JoinHolidays),
                Utils.eurliborConvention(tenor), Utils.eurliborEOM(tenor), new Actual360(), settings, h)
      {
         target_ = new TARGET();
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
      }

      /* Date calculations

            See <https://www.theice.com/marketdata/reports/170>.
      */
      public override Date valueDate(Date fixingDate)
      {

         Utils.QL_REQUIRE(isValidFixingDate(fixingDate), () => "Fixing date " + fixingDate + " is not valid");

         // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
         // In the case of EUR the Value Date shall be two TARGET
         // business days after the Fixing Date.
         return target_.advance(fixingDate, fixingDays_, TimeUnit.Days);
      }
      public override Date maturityDate(Date valueDate)
      {
         // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
         // In the case of EUR only, maturity dates will be based on days in
         // which the Target system is open.
         return target_.advance(valueDate, tenor_, convention_, endOfMonth());
      }
   }

   //! base class for the one day deposit ICE %EUR %LIBOR indexes
   /*! Euro O/N LIBOR fixed by ICE. It can be also used for T/N and S/N
       indexes, even if such indexes do not have ICE fixing.

       See <https://www.theice.com/marketdata/reports/170>.

       \warning This is the rate fixed in London by ICE. Use Eonia if
                you're interested in the fixing by the ECB.
   */
   public class DailyTenorEURLibor : IborIndex
   {

      // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
      // no o/n or s/n fixings (as the case may be) will take place
      // when the principal centre of the currency concerned is
      // closed but London is open on the fixing day.
      public DailyTenorEURLibor(int settlementDays, Settings settings)
         : this(settlementDays, settings, new Handle<YieldTermStructure>())
      {}

      public DailyTenorEURLibor(Settings settings)
         : base("EURLibor", new Period(1, TimeUnit.Days), 0, new EURCurrency(), new TARGET(),
                Utils.eurliborConvention(new Period(1, TimeUnit.Days)), Utils.eurliborEOM(new Period(1, TimeUnit.Days)), new Actual360(), settings, new Handle<YieldTermStructure>())
      {}

      public DailyTenorEURLibor(int settlementDays, Settings settings, Handle<YieldTermStructure> h)
         : base("EURLibor", new Period(1, TimeUnit.Days), settlementDays, new EURCurrency(), new TARGET(),
                Utils.eurliborConvention(new Period(1, TimeUnit.Days)), Utils.eurliborEOM(new Period(1, TimeUnit.Days)), new Actual360(), settings, h)
      {}

   }

   //! Overnight %EUR %Libor index
   public class EURLiborON : DailyTenorEURLibor
   {
      public EURLiborON(Settings settings)
         : base(0, settings, new Handle<YieldTermStructure>())
      {}

      public EURLiborON(Settings settings, Handle<YieldTermStructure> h)
         : base(0, settings, h)
      {}
   }

   //! 1-week %EUR %Libor index
   public class EURLiborSW : EURLibor
   {
      public EURLiborSW(Settings settings)
         : base(new Period(1, TimeUnit.Weeks), settings, new Handle<YieldTermStructure>())
      {}
      public EURLiborSW(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(1, TimeUnit.Weeks), settings, h)
      {}
   }

   //! 2-weeks %EUR %Libor index
   public class EURLibor2W : EURLibor
   {
      public EURLibor2W(Settings settings)
         : base(new Period(2, TimeUnit.Weeks), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor2W(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(2, TimeUnit.Weeks), settings, h)
      {}

   }


   //! 1-month %EUR %Libor index
   public class EURLibor1M : EURLibor
   {
      public EURLibor1M(Settings settings)
         : base(new Period(1, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor1M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(1, TimeUnit.Months), settings, h)
      {}

   }

   //! 2-months %EUR %Libor index
   public class EURLibor2M : EURLibor
   {
      public EURLibor2M(Settings settings)
         : base(new Period(2, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor2M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(2, TimeUnit.Months), settings, h)
      {}

   }

   //! 3-months %EUR %Libor index
   public class EURLibor3M : EURLibor
   {
      public EURLibor3M(Settings settings)
         : base(new Period(3, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor3M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(3, TimeUnit.Months), settings, h)
      {}

   }

   //! 4-months %EUR %Libor index
   public class EURLibor4M : EURLibor
   {
      public EURLibor4M(Settings settings)
         : base(new Period(4, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor4M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(4, TimeUnit.Months), settings, h)
      {}

   }

   //! 5-months %EUR %Libor index
   public class EURLibor5M : EURLibor
   {
      public EURLibor5M(Settings settings)
         : base(new Period(5, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor5M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(5, TimeUnit.Months), settings, h)
      {}

   }

   //! 6-months %EUR %Libor index
   public class EURLibor6M : EURLibor
   {
      public EURLibor6M(Settings settings)
         : base(new Period(6, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor6M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(6, TimeUnit.Months), settings, h)
      {}

   }

   //! 7-months %EUR %Libor index
   public class EURLibor7M : EURLibor
   {
      public EURLibor7M(Settings settings)
         : base(new Period(7, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor7M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(7, TimeUnit.Months), settings, h)
      {}

   }

   //! 8-months %EUR %Libor index
   public class EURLibor8M : EURLibor
   {
      public EURLibor8M(Settings settings)
         : base(new Period(8, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor8M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(8, TimeUnit.Months), settings, h)
      {}

   }

   //! 9-months %EUR %Libor index
   public class EURLibor9M : EURLibor
   {
      public EURLibor9M(Settings settings)
         : base(new Period(9, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor9M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(9, TimeUnit.Months), settings, h)
      {}

   }

   //! 10-months %EUR %Libor index
   public class EURLibor10M : EURLibor
   {
      public EURLibor10M(Settings settings)
         : base(new Period(10, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor10M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(10, TimeUnit.Months), settings, h)
      {}

   }

   //! 11-months %EUR %Libor index
   public class EURLibor11M : EURLibor
   {
      public EURLibor11M(Settings settings)
         : base(new Period(11, TimeUnit.Months), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor11M(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(11, TimeUnit.Months), settings, h)
      {}

   }

   //! 1-year %EUR %Libor index
   public class EURLibor1Y : EURLibor
   {
      public EURLibor1Y(Settings settings)
         : base(new Period(1, TimeUnit.Years), settings, new Handle<YieldTermStructure>())
      {}

      public EURLibor1Y(Settings settings, Handle<YieldTermStructure> h)
         : base(new Period(1, TimeUnit.Years), settings, h)
      {}

   }


}
