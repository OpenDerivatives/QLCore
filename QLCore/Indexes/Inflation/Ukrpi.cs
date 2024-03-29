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
   //! UK Retail Price Inflation Index
   public class UKRPI : ZeroInflationIndex
   {
      public UKRPI(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<ZeroInflationTermStructure>()) { }

      public UKRPI(bool interpolated, Settings settings, Handle<ZeroInflationTermStructure> ts)
         : base("RPI", new UKRegion(), false, interpolated, Frequency.Monthly,
                new Period(1, TimeUnit.Months), new GBPCurrency(), settings, ts) {}
   }

   //! Genuine year-on-year UK RPI (i.e. not a ratio of UK RPI)
   public class YYUKRPI : YoYInflationIndex
   {
      public YYUKRPI(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<YoYInflationTermStructure>()) { }

      public YYUKRPI(bool interpolated, Settings settings, Handle<YoYInflationTermStructure> ts)
         : base("YY_RPI", new UKRegion(), false, interpolated, false, Frequency.Monthly,
                new Period(1, TimeUnit.Months), new GBPCurrency(), settings, ts) {}
   }

   //! Fake year-on-year UK RPI (i.e. a ratio of UK RPI)
   public class YYUKRPIr : YoYInflationIndex
   {
      public YYUKRPIr(bool interpolated, Settings settings)
         : this(interpolated, settings, new Handle<YoYInflationTermStructure>()) { }

      public YYUKRPIr(bool interpolated, Settings settings, Handle<YoYInflationTermStructure> ts)
         : base("YYR_RPI", new UKRegion(), false, interpolated, true, Frequency.Monthly,
                new Period(1, TimeUnit.Months), new GBPCurrency(), settings, ts) {}
   }
}
