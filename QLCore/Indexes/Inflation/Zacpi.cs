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
   //! South African CPI index
   public class ZACPI : ZeroInflationIndex
   {
      public ZACPI(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<ZeroInflationTermStructure>()) { }

      public ZACPI(bool interpolated, Settings settings,
                   Handle<ZeroInflationTermStructure> ts)
         : base("CPI",
                new ZARegion(),
                false,
                interpolated,
                Frequency.Monthly,
                new Period(1, TimeUnit.Months),   // availability
                new ZARCurrency(),
                settings,
                ts) { }
   }

   //! Genuine year-on-year South African CPI (i.e. not a ratio of South African CPI)
   public class YYZACPI : YoYInflationIndex
   {
      public YYZACPI(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<YoYInflationTermStructure>()) { }

      public YYZACPI(bool interpolated, Settings settings,
                     Handle<YoYInflationTermStructure> ts)
         : base("YY_CPI",
                new ZARegion(),
                false,
                interpolated,
                false,
                Frequency.Monthly,
                new Period(1, TimeUnit.Months),
                new ZARCurrency(),
                settings,
                ts) { }
   }

   //! Fake year-on-year South African CPI (i.e. a ratio of South African CPI)
   public class YYZACPIr : YoYInflationIndex
   {
      public YYZACPIr(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<YoYInflationTermStructure>()) { }

      public YYZACPIr(bool interpolated, Settings settings,
                      Handle<YoYInflationTermStructure> ts)
         : base("YYR_CPI",
                new ZARegion(),
                false,
                interpolated,
                true,
                Frequency.Monthly,
                new Period(1, TimeUnit.Months),
                new ZARCurrency(),
                settings,
                ts) { }
   }
}