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

   //! %GBP %LIBOR rate
   /*! Pound Sterling LIBOR fixed by ICE.

       See <https://www.theice.com/marketdata/reports/170>.
   */
   public class GBPLibor : Libor
   {
      public GBPLibor(Period tenor)
         : base("GBPLibor", tenor, 0, new GBPCurrency(), new UnitedKingdom(UnitedKingdom.Market.Exchange), new Actual365Fixed(),
                new Handle<YieldTermStructure>())
      {}

      public GBPLibor(Period tenor, Handle<YieldTermStructure> h)
         : base("GBPLibor", tenor, 0, new GBPCurrency(), new UnitedKingdom(UnitedKingdom.Market.Exchange), new Actual365Fixed(), h)
      {}

   }

   //! Base class for the one day deposit ICE %GBP %LIBOR indexes
   public class DailyTenorGBPLibor : DailyTenorLibor
   {
      public DailyTenorGBPLibor(int settlementDays, Handle<YieldTermStructure> h)
         : base("GBPLibor", settlementDays, new GBPCurrency(), new UnitedKingdom(UnitedKingdom.Market.Exchange),
                new Actual365Fixed(), h)
      {}
   }

   //! Overnight %GBP %Libor index
   public class GBPLiborON : DailyTenorGBPLibor
   {
      public GBPLiborON(Handle<YieldTermStructure> h) : base(0, h)
      {}
   }
}
