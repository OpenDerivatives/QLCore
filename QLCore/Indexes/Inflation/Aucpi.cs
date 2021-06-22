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
   //! AU CPI index (either quarterly or annual)
   public class AUCPI : ZeroInflationIndex
   {
      public AUCPI(Frequency frequency,
                   bool revised,
                   bool interpolated,
                   Settings settings)
         : this(frequency, revised, interpolated, settings, new Handle<ZeroInflationTermStructure>()) {}

      public AUCPI(Frequency frequency,
                   bool revised,
                   bool interpolated,
                   Settings settings,
                   Handle<ZeroInflationTermStructure> ts)
         : base("CPI",
                new AustraliaRegion(),
                revised,
                interpolated,
                frequency,
                new Period(2, TimeUnit.Months),
                new AUDCurrency(),
                settings,
                ts) {}

   }

   //! Genuine year-on-year AU CPI (i.e. not a ratio)
   public class YYAUCPI : YoYInflationIndex
   {
      public YYAUCPI(Frequency frequency,
                     bool revised,
                     bool interpolated,
                     Settings settings)
         : this(frequency, revised, interpolated, settings, new Handle<YoYInflationTermStructure>()) {}

      public YYAUCPI(Frequency frequency,
                     bool revised,
                     bool interpolated,
                     Settings settings,
                     Handle<YoYInflationTermStructure> ts)
         : base("YY_CPI",
                new AustraliaRegion(),
                revised,
                interpolated,
                false,
                frequency,
                new Period(2, TimeUnit.Months),
                new AUDCurrency(),
                settings,
                ts) {}
   }


   //! Fake year-on-year AUCPI (i.e. a ratio)
   public class YYAUCPIr : YoYInflationIndex
   {
      public YYAUCPIr(Frequency frequency,
                      bool revised,
                      bool interpolated,
                     Settings settings)
         : this(frequency, revised, interpolated, settings, new Handle<YoYInflationTermStructure>()) { }

      public YYAUCPIr(Frequency frequency,
                      bool revised,
                      bool interpolated,
                     Settings settings,
                      Handle<YoYInflationTermStructure> ts)
         : base("YYR_CPI",
                new AustraliaRegion(),
                revised,
                interpolated,
                true,
                frequency,
                new Period(2, TimeUnit.Months),
                new AUDCurrency(),
                settings,
                ts) {}
   }
}
