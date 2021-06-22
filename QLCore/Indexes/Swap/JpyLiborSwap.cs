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
   public class JpyLiborSwapIsdaFixAm : SwapIndex
   {
      public JpyLiborSwapIsdaFixAm(Period tenor, Settings settings)
         : this(tenor, settings, new Handle<YieldTermStructure>()) { }

      public JpyLiborSwapIsdaFixAm(Period tenor, Settings settings, Handle<YieldTermStructure> h)
         : base("JpyLiborSwapIsdaFixAm", // familyName
                tenor,
                2, // settlementDays
                new JPYCurrency(),
                new TARGET(),
                new Period(6, TimeUnit.Months), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new ActualActual(ActualActual.Convention.ISDA), // fixedLegDaycounter
                new JPYLibor(new Period(6, TimeUnit.Months), settings, h),
                settings) { }
   }

   public class JpyLiborSwapIsdaFixPm : SwapIndex
   {
      public JpyLiborSwapIsdaFixPm(Period tenor, Settings settings)
         : this(tenor, settings, new Handle<YieldTermStructure>()) { }

      public JpyLiborSwapIsdaFixPm(Period tenor, Settings settings, Handle<YieldTermStructure> h)
         : base("JpyLiborSwapIsdaFixPm", // familyName
                tenor,
                2, // settlementDays
                new JPYCurrency(),
                new TARGET(),
                new Period(6, TimeUnit.Months), // fixedLegTenor
                BusinessDayConvention.ModifiedFollowing, // fixedLegConvention
                new ActualActual(ActualActual.Convention.ISDA), // fixedLegDaycounter
                new JPYLibor(new Period(6, TimeUnit.Months), settings, h),
                settings) { }
   }
}
