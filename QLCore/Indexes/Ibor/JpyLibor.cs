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

   //! %JPY %LIBOR rate
   /*! Japanese Yen LIBOR fixed by ICE.

       See <https://www.theice.com/marketdata/reports/170>.

       \warning This is the rate fixed in London by ICE. Use TIBOR if
                you're interested in the Tokio fixing.
   */
   public class JPYLibor : Libor
   {
      public JPYLibor(Period tenor)
         : base("JPYLibor", tenor, 2, new JPYCurrency(), new Japan(), new Actual360(), new Handle<YieldTermStructure>())
      {}

      public JPYLibor(Period tenor, Handle<YieldTermStructure> h)
         : base("JPYLibor", tenor, 2, new JPYCurrency(), new Japan(), new Actual360(), h)
      {}

   }

   //! base class for the one day deposit ICE %JPY %LIBOR indexes
   public class DailyTenorJPYLibor : DailyTenorLibor
   {
      public DailyTenorJPYLibor(int settlementDays) : this(settlementDays, new Handle<YieldTermStructure>())
      {}

      public DailyTenorJPYLibor(int settlementDays, Handle<YieldTermStructure> h)
         : base("JPYLibor", settlementDays, new JPYCurrency(), new Japan(), new Actual360(), h)
      {}
   }
}
