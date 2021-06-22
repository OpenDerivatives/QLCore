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

using System.Collections.Generic;

namespace QLCore
{
   /*! Abstract base class interface for a (double indexed) vector of (strike
       indexed) optionlet (i.e. caplet/floorlet) volatilities.
   */
   public abstract class StrippedOptionletBase : LazyObject
   {
      public abstract List<double> optionletStrikes(int i) ;
      public abstract List<double> optionletVolatilities(int i) ;

      public abstract List<Date> optionletFixingDates() ;
      public abstract List<double> optionletFixingTimes() ;
      public abstract int optionletMaturities() ;

      public abstract List<double> atmOptionletRates() ;

      public abstract DayCounter dayCounter() ;
      public abstract Calendar calendar() ;
      public abstract int settlementDays() ;
      public abstract BusinessDayConvention businessDayConvention() ;
      public abstract VolatilityType volatilityType() ;
      public abstract double displacement() ;
      public abstract Settings settings() ;
      public abstract void setSettings(Settings s) ;
   }
}
