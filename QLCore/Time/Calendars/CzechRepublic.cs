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
   //! Czech calendars
   /*! Holidays for the Prague stock exchange (see http://www.pse.cz/):
       <ul>
       <li>Saturdays</li>
       <li>Sundays</li>
       <li>New Year's Day, January 1st</li>
       <li>Easter Monday</li>
       <li>Labour Day, May 1st</li>
       <li>Liberation Day, May 8th</li>
       <li>SS. Cyril and Methodius, July 5th</li>
       <li>Jan Hus Day, July 6th</li>
       <li>Czech Statehood Day, September 28th</li>
       <li>Independence Day, October 28th</li>
       <li>Struggle for Freedom and Democracy Day, November 17th</li>
       <li>Christmas Eve, December 24th</li>
       <li>Christmas, December 25th</li>
       <li>St. Stephen, December 26th</li>
       </ul>

       \ingroup calendars
   */
   public class CzechRepublic : Calendar
   {
      public CzechRepublic() : base(Impl.Singleton) { }

      class Impl : Calendar.WesternImpl
      {
         public static readonly Impl Singleton = new Impl();
         private Impl() { }

         public override string name() { return "Prague stock exchange"; }
         public override bool isBusinessDay(Date date)
         {
            DayOfWeek w = date.DayOfWeek;
            int d = date.Day, dd = date.DayOfYear;
            Month m = (Month)date.Month;
            int y = date.Year;
            int em = easterMonday(y);

            if (isWeekend(w)
                // New Year's Day
                || (d == 1 && m == Month.January)
                // Easter Monday
                || (dd == em)
                // Labour Day
                || (d == 1 && m == Month.May)
                // Liberation Day
                || (d == 8 && m == Month.May)
                // SS. Cyril and Methodius
                || (d == 5 && m == Month.July)
                // Jan Hus Day
                || (d == 6 && m == Month.July)
                // Czech Statehood Day
                || (d == 28 && m == Month.September)
                // Independence Day
                || (d == 28 && m == Month.October)
                // Struggle for Freedom and Democracy Day
                || (d == 17 && m == Month.November)
                // Christmas Eve
                || (d == 24 && m == Month.December)
                // Christmas
                || (d == 25 && m == Month.December)
                // St. Stephen
                || (d == 26 && m == Month.December)
                // unidentified closing days for stock exchange
                || (d == 2 && m == Month.January && y == 2004)
                || (d == 31 && m == Month.December && y == 2004))
               return false;
            return true;
         }
      }
   }
}
