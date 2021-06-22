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

using System;

namespace QLCore
{
   //! Volatility term structure
   /*! This abstract class defines the interface of concrete
       volatility structures which will be derived from this one.

   */
   public abstract class VolatilityTermStructure : TermStructure
   {
      #region Constructors

      /*! \warning term structures initialized by means of this
                   constructor must manage their own reference date
                   by overriding the referenceDate() method.
      */

      protected VolatilityTermStructure(Settings settings, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, dc)
      {
         bdc_ = bdc;
      }
      //! initialize with a fixed reference date
      protected VolatilityTermStructure(Settings settings, Date referenceDate, Calendar cal, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, referenceDate, cal, dc)
      {
         bdc_ = bdc;
      }
      //! calculate the reference date based on the global evaluation date
      protected VolatilityTermStructure(Settings settings, int settlementDays, Calendar cal, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, settlementDays, cal, dc)
      {
         bdc_ = bdc;
      }

      #endregion

      //! the business day convention used in tenor to date conversion
      public virtual BusinessDayConvention businessDayConvention() {return bdc_;}

      //! period/date conversion
      public virtual Date optionDateFromTenor(Period p)
      {
         // swaption style
         return calendar().advance(referenceDate(), p, businessDayConvention());
      }

      //! the minimum strike for which the term structure can return vols
      public abstract double minStrike();

      //! the maximum strike for which the term structure can return vols
      public abstract double maxStrike();

      //! strike-range check
      protected void checkStrike(double k, bool extrapolate)
      {
         Utils.QL_REQUIRE(extrapolate || allowsExtrapolation() ||
                          (k >= minStrike() && k <= maxStrike()), () =>
                          "strike (" + k + ") is outside the curve domain ["
                          + minStrike() + "," + maxStrike() + "]");
      }

      private BusinessDayConvention bdc_;

   }
}
