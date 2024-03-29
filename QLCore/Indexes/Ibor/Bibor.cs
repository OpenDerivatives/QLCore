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
   /// <summary>
   /// Bibor index
   /// Bangkok Interbank Offered Rate  fixed by the Bank of Thailand BOT.
   /// </summary>
   public class Bibor : IborIndex
   {
      public static BusinessDayConvention BiborConvention(Period p)
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

      public static bool BiborEOM(Period p)
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
               Utils.QL_FAIL("invalid time units");
               return false;
         }
      }

      public Bibor(Period tenor, Settings settings, Handle<YieldTermStructure> h = null)
         : base("Bibor", tenor, 2, new THBCurrency(), new Thailand(),
                BiborConvention(tenor), BiborEOM(tenor),
                new Actual365Fixed(), settings, h ?? new Handle<YieldTermStructure>())
      {
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
      }

      /// <summary>
      /// 1-week Bibor index
      /// </summary>
      public class BiborSW : Bibor
      {
         public BiborSW(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(1, TimeUnit.Weeks), settings, h ?? new Handle<YieldTermStructure>())
         {}
      }

      /// <summary>
      /// 1-month Bibor index
      /// </summary>
      public class Bibor1M : Bibor
      {
         public Bibor1M(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(1, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
         {}
      }

      /// <summary>
      /// 2-months Bibor index
      /// </summary>
      public class Bibor2M : Bibor
      {
         public Bibor2M(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(2, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
         { }
      }

      /// <summary>
      /// 3-months Bibor index
      /// </summary>
      public class Bibor3M : Bibor
      {
         public Bibor3M(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(3, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
         { }
      }

      /// <summary>
      /// 6-months Bibor index
      /// </summary>
      public class Bibor6M : Bibor
      {
         public Bibor6M(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(6, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
         { }
      }

      /// <summary>
      /// 9-months Bibor index
      /// </summary>
      public class Bibor9M : Bibor
      {
         public Bibor9M(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(9, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
         { }
      }

      /// <summary>
      /// 1-year Bibor index
      /// </summary>
      public class Bibor1Y : Bibor
      {
         public Bibor1Y(Settings settings, Handle<YieldTermStructure> h = null)
            : base(new Period(1, TimeUnit.Years), settings, h ?? new Handle<YieldTermStructure>())
         { }
      }
   }
}
