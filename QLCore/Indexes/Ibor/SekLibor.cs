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

   //! %SEK %LIBOR rate
   /*! Sweden Krone LIBOR discontinued as of 2013.
   */
   public class SEKLibor : Libor
   {
      public SEKLibor(Period tenor, Settings settings)
         : base("SEKLibor", tenor, 2, new SEKCurrency(), new Sweden(), new Actual360(), settings, new Handle<YieldTermStructure>())
      {}

      public SEKLibor(Period tenor, Settings settings, Handle<YieldTermStructure> h)
         : base("SEKLibor", tenor, 2, new SEKCurrency(), new Sweden(), new Actual360(), settings, h)
      {}

   }

}
