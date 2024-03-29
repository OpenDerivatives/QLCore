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
   //! base class for all ICE LIBOR indexes but the EUR, O/N, and S/N ones
   /*! LIBOR fixed by ICE.

       See <https://www.theice.com/marketdata/reports/170>.
   */
   public class Libor : IborIndex
   {
      private Calendar financialCenterCalendar_;
      private Calendar jointCalendar_;

      public Libor(string familyName, Period tenor, int settlementDays, Currency currency, Calendar financialCenterCalendar,
                   DayCounter dayCounter, Settings settings, Handle<YieldTermStructure> h)
         : base(familyName, tenor, settlementDays, currency,
                // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
                // UnitedKingdom::Exchange is the fixing calendar for
                // a) all currencies but EUR
                // b) all indexes but o/n and s/n
                new UnitedKingdom(UnitedKingdom.Market.Exchange),
                Utils.liborConvention(tenor), Utils.liborEOM(tenor),
                dayCounter, settings, h)
      {

         financialCenterCalendar_ = financialCenterCalendar;
         jointCalendar_ = new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange),
                                            financialCenterCalendar, JointCalendar.JointCalendarRule.JoinHolidays);
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
         Utils.QL_REQUIRE(currency != new EURCurrency(), () =>
                          "for EUR Libor dedicated EurLibor constructor must be used");
      }

      /* Date calculations

            See <https://www.theice.com/marketdata/reports/170>.
      */
      public override Date valueDate(Date fixingDate)
      {
         Utils.QL_REQUIRE(isValidFixingDate(fixingDate), () => "Fixing date " + fixingDate + " is not valid");

         // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
         // For all currencies other than EUR and GBP the period between
         // Fixing Date and Value Date will be two London business days
         // after the Fixing Date, or if that day is not both a London
         // business day and a business day in the principal financial centre
         // of the currency concerned, the next following day which is a
         // business day in both centres shall be the Value Date.
         Date d = fixingCalendar().advance(fixingDate, fixingDays_, TimeUnit.Days);
         return jointCalendar_.adjust(d);
      }

      public override Date maturityDate(Date valueDate)
      {
         // Where a deposit is made on the final business day of a
         // particular calendar month, the maturity of the deposit shall
         // be on the final business day of the month in which it matures
         // (not the corresponding date in the month of maturity). Or in
         // other words, in line with market convention, BBA LIBOR rates
         // are dealt on an end-end basis. For instance a one month
         // deposit for value 28th February would mature on 31st March,
         // not the 28th of March.
         return jointCalendar_.advance(valueDate, tenor_, convention_, endOfMonth());
      }

      // Other methods
      public override IborIndex clone(Handle<YieldTermStructure> h)
      {
         return new Libor(familyName(), tenor(), fixingDays(), currency(), financialCenterCalendar_,
                          dayCounter(), settings(), h);
      }
   }

   //! base class for all O/N-S/N BBA LIBOR indexes but the EUR ones
   /*! One day deposit LIBOR fixed by ICE.

       See <https://www.theice.com/marketdata/reports/170>.
   */
   public class DailyTenorLibor : IborIndex
   {
      // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
      // no o/n or s/n fixings (as the case may be) will take place
      // when the principal centre of the currency concerned is
      // closed but London is open on the fixing day.
      public DailyTenorLibor(string familyName, int settlementDays, Currency currency, Calendar financialCenterCalendar,
                             DayCounter dayCounter, Settings settings)
         : this(familyName, settlementDays, currency, financialCenterCalendar, dayCounter, settings, new Handle<YieldTermStructure>())
      {}

      public DailyTenorLibor(string familyName, int settlementDays, Currency currency, Calendar financialCenterCalendar,
                             DayCounter dayCounter, Settings settings, Handle<YieldTermStructure> h)
         : base(familyName, new Period(1, TimeUnit.Days), settlementDays, currency,
                new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), financialCenterCalendar, JointCalendar.JointCalendarRule.JoinHolidays),
                Utils.liborConvention(new Period(1, TimeUnit.Days)), Utils.liborEOM(new Period(1, TimeUnit.Days)), dayCounter, settings, h)
      {
         Utils.QL_REQUIRE(currency != new EURCurrency(), () =>
                          "for EUR Libor dedicated EurLibor constructor must be used");
      }
   }

   public static partial class Utils
   {
      public static BusinessDayConvention liborConvention(Period p)
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
               QL_FAIL("invalid time units");
               return BusinessDayConvention.Unadjusted;
         }
      }

      public static bool liborEOM(Period p)
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
               QL_FAIL("invalid time units");
               return false;
         }
      }
   }
}
