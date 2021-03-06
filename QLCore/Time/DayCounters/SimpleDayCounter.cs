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
   //! Simple day counter for reproducing theoretical calculations.
   /*! This day counter tries to ensure that whole-month distances are returned as a simple fraction, i.e., 1 year = 1.0, 6 months = 0.5, 3 months = 0.25 and so forth.
     this day counter should be used together with NullCalendar, which ensures that dates at whole-month distances share the same day of month. It is <b>not</b> guaranteed to work with any other calendar. */
   public class SimpleDayCounter : DayCounter
   {
      public SimpleDayCounter() : base(Impl.Singleton) { }

      class Impl : DayCounter
      {
         public static readonly Impl Singleton = new Impl();
         private Impl() { }

         public override string name() { return "Simple"; }
         public override int dayCount(Date d1, Date d2) { return Thirty360.US_Impl.Singleton.dayCount(d1, d2); }
         public override double yearFraction(Date d1, Date d2, Date d3, Date d4)
         {
            int dm1 = d1.Day,
                dm2 = d2.Day;

            if (dm1 == dm2 ||
                // e.g., Aug 30 -> Feb 28 ?
                (dm1 > dm2 && Date.isEndOfMonth(d2)) ||
                // e.g., Feb 28 -> Aug 30 ?
                (dm1 < dm2 && Date.isEndOfMonth(d1)))
            {

               return (d2.Year - d1.Year) + (d2.Month - d1.Month) / 12.0;
            }
            else
               return Thirty360.US_Impl.Singleton.yearFraction(d1, d2, d3, d4);
         }
      }
   }
}