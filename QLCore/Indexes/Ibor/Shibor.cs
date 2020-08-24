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

namespace QLCore
{
   public class Shibor : IborIndex
   {
      private static BusinessDayConvention shiborConvention(Period p)
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
               Utils.QL_FAIL("invalid time units");
               return BusinessDayConvention.Unadjusted;
         }
      }

      public Shibor(Period tenor)
         : this(tenor, new Handle<YieldTermStructure>()) {}

      public Shibor(Period tenor, Handle<YieldTermStructure> h)
         : base("Shibor", tenor, (tenor == new Period(1, TimeUnit.Days) ? 0 : 1), new CNYCurrency(),
                new China(China.Market.IB), shiborConvention(tenor), false,
                new Actual360(), h) {}
   }
}
