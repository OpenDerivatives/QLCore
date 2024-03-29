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
   //! Cap/floor term-volatility structure
   /*! This class is purely abstract and defines the interface of concrete
       structures which will be derived from this one.
   */
   public abstract class CapFloorTermVolatilityStructure : VolatilityTermStructure
   {
      #region Constructors
      /*! \warning term structures initialized by means of this
                   constructor must manage their own reference date
                   by overriding the referenceDate() method.
      */

      protected CapFloorTermVolatilityStructure(Settings settings, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, bdc, dc) {}

      //! initialize with a fixed reference date
      protected CapFloorTermVolatilityStructure(Settings settings, Date referenceDate, Calendar cal, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, referenceDate, cal, bdc, dc) {}

      //! calculate the reference date based on the global evaluation date
      protected CapFloorTermVolatilityStructure(Settings settings, int settlementDays, Calendar cal, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, settlementDays, cal, bdc, dc) {}

      #endregion

      #region Volatility

      //! returns the volatility for a given cap/floor length and strike rate
      public double volatility(Period length, double strike, bool extrapolate = false)
      {
         Date d = optionDateFromTenor(length);
         return volatility(d, strike, extrapolate);
      }

      public double volatility(Date end, double strike, bool extrapolate = false)
      {
         checkRange(end, extrapolate);
         double t = timeFromReference(end);
         return volatility(t, strike, extrapolate);
      }

      //! returns the volatility for a given end time and strike rate
      public double volatility(double t, double strike, bool extrapolate = false)
      {
         checkRange(t, extrapolate);
         checkStrike(strike, extrapolate);
         return volatilityImpl(t, strike);
      }

      #endregion

      //! implements the actual volatility calculation in derived classes
      protected abstract double volatilityImpl(double length, double strike);
   }
}
