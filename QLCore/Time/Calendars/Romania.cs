﻿/*
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
   //! Romanian calendars
   /*! Public holidays:
       <ul>
       <li>Saturdays</li>
       <li>Sundays</li>
       <li>New Year's Day, January 1st</li>
       <li> Day after New Year's Day, January 2nd</li>
       <li>Unification Day, January 24th</li>
       <li>Orthodox Easter (only Sunday and Monday)</li>
       <li>Labour Day, May 1st</li>
       <li>Pentecost with Monday (50th and 51st days after the
           Othodox Easter)</li>
       <li>St Marys Day, August 15th</li>
       <li>Feast of St Andrew, November 30th</li>
       <li>National Day, December 1st</li>
       <li>Christmas, December 25th</li>
       <li>2nd Day of Christmas, December 26th</li>
       </ul>

       \ingroup calendars
   */
   public class Romania : Calendar
   {
      public Romania() : base(Impl.Singleton) { }

      class Impl : Calendar.OrthodoxImpl
      {
         public static readonly Impl Singleton = new Impl();
         private Impl() { }

         public override string name() { return "Romania"; }
         public override bool isBusinessDay(Date date)
         {
            DayOfWeek w = date.DayOfWeek;
            int d = date.Day, dd = date.DayOfYear;
            Month m = (Month) date.Month;
            int y = date.Year;
            int em = easterMonday(y);
            if (isWeekend(w)
                // New Year's Day
                || (d == 1 && m == Month.January)
                // Day after New Year's Day
                || (d == 2 && m == Month.January)
                // Unification Day
                || (d == 24 && m == Month.January)
                // Orthodox Easter Monday
                || (dd == em)
                // Labour Day
                || (d == 1 && m == Month.May)
                // Pentecost
                || (dd == em + 49)
                // St Marys Day
                || (d == 15 && m == Month.August)
                // Feast of St Andrew
                || (d == 30 && m == Month.November)
                // National Day
                || (d == 1 && m == Month.December)
                // Christmas
                || (d == 25 && m == Month.December)
                // 2nd Day of Chritsmas
                || (d == 26 && m == Month.December))
               return false;
            return true;
         }
      }
   }
}
