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
   //! %Bkbm index
   /*! Bkbm rate fixed by NZFMA.

       See <http://www.nzfma.org/Site/data/default.aspx>.
   */
   public class Bkbm : IborIndex
   {
      public Bkbm(Period tenor, Settings settings, Handle<YieldTermStructure> h = null)
         : base("Bkbm", tenor,
                0, // settlement days
                new NZDCurrency(), new NewZealand(),
                BusinessDayConvention.ModifiedFollowing, true,
                new Actual365Fixed(), settings, h ?? new Handle<YieldTermStructure>())
      {
         Utils.QL_REQUIRE(this.tenor().units() != TimeUnit.Days, () =>
                          "for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
      }
   }

   //! 1-month %Bkbm index
   public class Bkbm1M : Bkbm
   {
      public Bkbm1M(Settings settings, Handle<YieldTermStructure> h = null)
         : base(new Period(1, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
      {}
   }

   //! 2-month %Bkbm index
   public class Bkbm2M : Bkbm
   {
      public Bkbm2M(Settings settings, Handle<YieldTermStructure> h = null)
         : base(new Period(2, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
      { }
   }

   //! 3-month %Bkbm index
   public class Bkbm3M : Bkbm
   {
      public Bkbm3M(Settings settings, Handle<YieldTermStructure> h = null)
         : base(new Period(3, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
      { }
   }

   //! 4-month %Bkbm index
   public class Bkbm4M : Bkbm
   {
      public Bkbm4M(Settings settings, Handle<YieldTermStructure> h = null)
         : base(new Period(4, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
      { }
   }

   //! 5-month %Bkbm index
   public class Bkbm5M : Bkbm
   {
      public Bkbm5M(Settings settings, Handle<YieldTermStructure> h = null)
         : base(new Period(5, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
      { }
   }

   //! 6-month %Bkbm index
   public class Bkbm6M : Bkbm
   {
      public Bkbm6M(Settings settings, Handle<YieldTermStructure> h = null)
         : base(new Period(6, TimeUnit.Months), settings, h ?? new Handle<YieldTermStructure>())
      { }
   }
}
