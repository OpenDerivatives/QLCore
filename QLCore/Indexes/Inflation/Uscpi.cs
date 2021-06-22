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
   //! US CPI index
   public class USCPI : ZeroInflationIndex
   {
      public USCPI(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<ZeroInflationTermStructure>()) {}

      public USCPI(bool interpolated, Settings settings,
                   Handle<ZeroInflationTermStructure> ts)
         : base("CPI",
                new USRegion(),
                false,
                interpolated,
                Frequency.Monthly,
                new Period(1, TimeUnit.Months), // availability
                new USDCurrency(),
                settings,
                ts) {}
   }

   //! Genuine year-on-year US CPI (i.e. not a ratio of US CPI)
   public class YYUSCPI : YoYInflationIndex
   {
      public YYUSCPI(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<YoYInflationTermStructure>()) {}

      public YYUSCPI(bool interpolated, Settings settings,
                     Handle<YoYInflationTermStructure> ts)
         : base("YY_CPI",
                new USRegion(),
                false,
                interpolated,
                false,
                Frequency.Monthly,
                new Period(1, TimeUnit.Months),
                new USDCurrency(),
                settings,
                ts) {}
   }

   //! Fake year-on-year US CPI (i.e. a ratio of US CPI)
   public class YYUSCPIr : YoYInflationIndex
   {
      public YYUSCPIr(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<YoYInflationTermStructure>()) { }

      public YYUSCPIr(bool interpolated, Settings settings,
                      Handle<YoYInflationTermStructure> ts)
         : base("YYR_CPI",
                new USRegion(),
                false,
                interpolated,
                true,
                Frequency.Monthly,
                new Period(1, TimeUnit.Months),
                new USDCurrency(),
                settings,
                ts) {}
   }
}
