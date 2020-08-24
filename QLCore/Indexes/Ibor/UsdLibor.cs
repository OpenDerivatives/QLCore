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
   //! %USD %LIBOR rate
   /*! US Dollar LIBOR fixed by ICE.

       See <https://www.theice.com/marketdata/reports/170>.
   */
   public class USDLibor : Libor
   {
      public USDLibor(Period tenor) : this(tenor, new Handle<YieldTermStructure>())
      {}

      public USDLibor(Period tenor, Handle<YieldTermStructure> h)
         : base("USDLibor", tenor, 2, new USDCurrency(), new UnitedStates(UnitedStates.Market.Settlement), new Actual360(), h)
      {}

   }

   //! base class for the one day deposit ICE %USD %LIBOR indexes
   public class DailyTenorUSDLibor : DailyTenorLibor
   {
      public DailyTenorUSDLibor(int settlementDays) : this(settlementDays, new Handle<YieldTermStructure>())
      {}
      public DailyTenorUSDLibor(int settlementDays, Handle<YieldTermStructure> h)
         : base("USDLibor", settlementDays, new USDCurrency(), new UnitedStates(UnitedStates.Market.Settlement), new Actual360(), h)
      {}

   }

   //! Overnight %USD %Libor index
   public class USDLiborON : DailyTenorUSDLibor
   {
      public USDLiborON() : this(new Handle<YieldTermStructure>())
      {}

      public USDLiborON(Handle<YieldTermStructure> h) : base(0, h)
      {}

   }
}
