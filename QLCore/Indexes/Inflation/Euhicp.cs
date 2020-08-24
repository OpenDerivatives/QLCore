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
   //! EU HICP index
   public class EUHICP : ZeroInflationIndex
   {
      public EUHICP(bool interpolated)
         : this(interpolated, new Handle<ZeroInflationTermStructure>()) { }

      public EUHICP(bool interpolated, Handle<ZeroInflationTermStructure> ts)
         : base("HICP", new EURegion(), false, interpolated, Frequency.Monthly,
                new Period(1, TimeUnit.Months), // availability
                new EURCurrency(), ts) {}
   }

   //! Genuine year-on-year EU HICP (i.e. not a ratio of EU HICP)
   public class YYEUHICP : YoYInflationIndex
   {
      public YYEUHICP(bool interpolated)
         : this(interpolated, new Handle<YoYInflationTermStructure>()) { }

      public YYEUHICP(bool interpolated, Handle<YoYInflationTermStructure> ts)
         : base("YY_HICP", new EURegion(), false, interpolated, false, Frequency.Monthly,
                new Period(1, TimeUnit.Months), new EURCurrency(), ts) {}
   }


   //! Fake year-on-year EU HICP (i.e. a ratio of EU HICP)
   public class YYEUHICPr : YoYInflationIndex
   {
      public YYEUHICPr(bool interpolated)
         : this(interpolated, new Handle<YoYInflationTermStructure>()) { }

      public YYEUHICPr(bool interpolated, Handle<YoYInflationTermStructure> ts)
         : base("YYR_HICP", new EURegion(), false, interpolated, true, Frequency.Monthly,
                new Period(1, TimeUnit.Months), new EURCurrency(), ts) {}
   }

}
