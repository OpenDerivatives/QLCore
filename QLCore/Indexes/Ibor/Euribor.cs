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
   public static partial class Utils
   {
      public static BusinessDayConvention euriborConvention(Period p)
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
               throw new ArgumentException("Unknown TimeUnit: " + p.units());
         }
      }

      public static bool euriborEOM(Period p)
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
               throw new ArgumentException("Unknown TimeUnit: " + p.units());
         }
      }
   }
   /// <summary>
   /// %Euribor index
   /// Euribor rate fixed by the ECB.
   /// This is the rate fixed by the ECB. Use EurLibor if you're interested in the London fixing by BBA.
   /// </summary>
   public class Euribor : IborIndex
   {
      public Euribor(Period tenor, Settings settings) : this(tenor, settings, new Handle<YieldTermStructure>()) { }
      public Euribor(Period tenor, Settings settings, Handle<YieldTermStructure> h) :
         base("Euribor", tenor, 2, // settlementDays
              new EURCurrency(), new TARGET(),
              Utils.euriborConvention(tenor), Utils.euriborEOM(tenor),
              new Actual360(), settings, h)
      {
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
      }
   }

   //! Actual/365 %Euribor index
   /*! Euribor rate adjusted for the mismatch between the actual/360
       convention used for Euribor and the actual/365 convention
       previously used by a few pre-EUR currencies.
   */
   public class Euribor365 : IborIndex
   {
      public Euribor365(Period tenor, Settings settings) : this(tenor, settings, new Handle<YieldTermStructure>()) { }
      public Euribor365(Period tenor, Settings settings, Handle<YieldTermStructure> h)
         : base("Euribor365", tenor,
                2, // settlement days
                new EURCurrency(), new TARGET(), Utils.euriborConvention(tenor), Utils.euriborEOM(tenor),
                new Actual365Fixed(), settings, h)
      {
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
      }
   }

   //! 1-week %Euribor index
   public class EuriborSW : Euribor
   {
      public EuriborSW(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public EuriborSW(Settings settings, Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Weeks), settings, h) { }
   }

   //! 2-weeks %Euribor index
   public class Euribor2W : Euribor
   {
      public Euribor2W(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor2W(Settings settings, Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Weeks), settings, h) { }
   }

   //! 3-weeks %Euribor index
   public class Euribor3W : Euribor
   {
      public Euribor3W(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor3W(Settings settings, Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Weeks), settings, h) { }
   }

   //! 1-month %Euribor index
   public class Euribor1M : Euribor
   {
      public Euribor1M(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor1M(Settings settings, Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Months), settings, h) { }
   }

   //! 2-months %Euribor index
   public class Euribor2M : Euribor
   {
      public Euribor2M(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor2M(Settings settings, Handle<YieldTermStructure> h) : base(new Period(2, TimeUnit.Months), settings, h) { }
   }

   // 3-months %Euribor index
   public class Euribor3M : Euribor
   {
      public Euribor3M(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor3M(Settings settings, Handle<YieldTermStructure> h) : base(new Period(3, TimeUnit.Months), settings, h) {}
   }

   // 4-months %Euribor index
   public class Euribor4M : Euribor
   {
      public Euribor4M(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor4M(Settings settings, Handle<YieldTermStructure> h) : base(new Period(4, TimeUnit.Months), settings, h) { }
   }

   // 5-months %Euribor index
   public class Euribor5M : Euribor
   {
      public Euribor5M(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor5M(Settings settings, Handle<YieldTermStructure> h) : base(new Period(5, TimeUnit.Months), settings, h) { }
   }

   // 6-months %Euribor index
   public class Euribor6M : Euribor
   {
      public Euribor6M(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor6M(Settings settings, Handle<YieldTermStructure> h) : base(new Period(6, TimeUnit.Months), settings, h) { }
   }

   // 1-year %Euribor index
   public class Euribor1Y : Euribor
   {
      public Euribor1Y(Settings settings) : this(settings, new Handle<YieldTermStructure>()) { }
      public Euribor1Y(Settings settings, Handle<YieldTermStructure> h) : base(new Period(1, TimeUnit.Years), settings, h) { }

   }

}
