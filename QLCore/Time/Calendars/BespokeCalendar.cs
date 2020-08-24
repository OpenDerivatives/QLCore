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
using System.Collections.Generic;

namespace QLCore
{
   //! Bespoke calendar
   /*! This calendar has no predefined set of business days. Holidays
       and weekdays can be defined by means of the provided
       interface. Instances constructed by copying remain linked to
       the original one; adding a new holiday or weekday will affect
       all linked instances.

       \ingroup calendars
   */
   public class BespokeCalendar : Calendar
   {
      private string name_;
      public override string name() { return name_; }

      /*! \warning different bespoke calendars created with the same
                   name (or different bespoke calendars created with
                   no name) will compare as equal.
      */
      public BespokeCalendar() : this("") { }
      public BespokeCalendar(string name) : base(new Impl())
      {
         name_ = name;
      }

      //! marks the passed day as part of the weekend
      public void addWeekend(DayOfWeek w)
      {
         Impl impl = calendar_ as Impl;
         if (impl != null)
            impl.addWeekend(w);
      }

      // here implementation does not follow a singleton pattern
      class Impl : Calendar.WesternImpl
      {
         public override bool isWeekend(DayOfWeek w) { return (weekend_.Contains(w)); }
         public override bool isBusinessDay(Date date) { return !isWeekend(date.DayOfWeek); }
         public void addWeekend(DayOfWeek w) { weekend_.Add(w); }

         private List<DayOfWeek> weekend_ = new List<DayOfWeek>();
      }
   }
}
