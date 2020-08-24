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
   //! Icelandic calendars
   /*! Holidays for the Iceland stock exchange
       (data from <http://www.icex.is/is/calendar?languageID=1>):
       <ul>
       <li>Saturdays</li>
       <li>Sundays</li>
       <li>New Year's Day, January 1st (possibly moved to Monday)</li>
       <li>Holy Thursday</li>
       <li>Good Friday</li>
       <li>Easter Monday</li>
       <li>First day of Summer (third or fourth Thursday in April)</li>
       <li>Labour Day, May 1st</li>
       <li>Ascension Thursday</li>
       <li>Pentecost Monday</li>
       <li>Independence Day, June 17th</li>
       <li>Commerce Day, first Monday in August</li>
       <li>Christmas, December 25th</li>
       <li>Boxing Day, December 26th</li>
       </ul>

       \ingroup calendars
   */
   public class Iceland : Calendar
   {
      public Iceland() : base(Impl.Singleton) { }

      class Impl : Calendar.WesternImpl
      {
         public static readonly Impl Singleton = new Impl();
         private Impl() { }

         public override string name() { return "Iceland stock exchange"; }
         public override bool isBusinessDay(Date date)
         {
            DayOfWeek w = date.DayOfWeek;
            int d = date.Day, dd = date.DayOfYear;
            Month m = (Month)date.Month;
            int y = date.Year;
            int em = easterMonday(y);

            if (isWeekend(w)
                // New Year's Day (possibly moved to Monday)
                || ((d == 1 || ((d == 2 || d == 3) && w == DayOfWeek.Monday))
                    && m == Month.January)
                // Holy Thursday
                || (dd == em - 4)
                // Good Friday
                || (dd == em - 3)
                // Easter Monday
                || (dd == em)
                // First day of Summer
                || (d >= 19 && d <= 25 && w == DayOfWeek.Thursday && m == Month.April)
                // Ascension Thursday
                || (dd == em + 38)
                // Pentecost Monday
                || (dd == em + 49)
                // Labour Day
                || (d == 1 && m == Month.May)
                // Independence Day
                || (d == 17 && m == Month.June)
                // Commerce Day
                || (d <= 7 && w == DayOfWeek.Monday && m == Month.August)
                // Christmas
                || (d == 25 && m == Month.December)
                // Boxing Day
                || (d == 26 && m == Month.December))
               return false;
            return true;
         }
      }
   }
}
