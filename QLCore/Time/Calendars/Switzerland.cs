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

   //! Swiss calendar
   /*! Holidays:
       <ul>
       <li>Saturdays</li>
       <li>Sundays</li>
       <li>New Year's Day, January 1st</li>
       <li>Berchtoldstag, January 2nd</li>
       <li>Good Friday</li>
       <li>Easter Monday</li>
       <li>Ascension Day</li>
       <li>Whit Monday</li>
       <li>Labour Day, May 1st</li>
       <li>National Day, August 1st</li>
       <li>Christmas, December 25th</li>
       <li>St. Stephen's Day, December 26th</li>
       </ul>

       \ingroup calendars
   */
   public class Switzerland : Calendar
   {
      public Switzerland() : base(Impl.Singleton) { }

      class Impl : Calendar.WesternImpl
      {
         public static readonly Impl Singleton = new Impl();
         private Impl() { }

         public override string name() { return "Switzerland"; }
         public override bool isBusinessDay(Date date)
         {
            DayOfWeek w = date.DayOfWeek;
            int d = date.Day, dd = date.DayOfYear;
            Month m = (Month)date.Month;
            int y = date.Year;
            int em = easterMonday(y);

            if (isWeekend(w)
                // New Year's Day
                || (d == 1  && m == Month.January)
                // Berchtoldstag
                || (d == 2  && m == Month.January)
                // Good Friday
                || (dd == em - 3)
                // Easter Monday
                || (dd == em)
                // Ascension Day
                || (dd == em + 38)
                // Whit Monday
                || (dd == em + 49)
                // Labour Day
                || (d == 1  && m == Month.May)
                // National Day
                || (d == 1  && m == Month.August)
                // Christmas
                || (d == 25 && m == Month.December)
                // St. Stephen's Day
                || (d == 26 && m == Month.December))
               return false;
            return true;
         }
      }
   }
}


