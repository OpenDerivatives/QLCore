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
   //! 1/1 day count convention
   public class OneDayCounter : DayCounter
   {
      public OneDayCounter() : base(Impl.Singleton) { }

      class Impl : DayCounter
      {
         public static readonly Impl Singleton = new Impl();
         private Impl() { }

         public override string name() { return "1/1"; }
         public override int dayCount(Date d1, Date d2)
         {
            // the sign is all we need
            return (d2 >= d1 ? 1 : -1);
         }

         public override double yearFraction(Date d1, Date d2, Date refPeriodStart, Date refPeriodEnd)
         {
            return dayCount(d1, d2);
         }
      }
   }
}