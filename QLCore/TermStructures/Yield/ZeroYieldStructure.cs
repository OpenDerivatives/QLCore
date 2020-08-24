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
using System.Collections.Generic;

namespace QLCore
{
   //! Zero-yield term structure
   /*! This abstract class acts as an adapter to YieldTermStructure
      allowing the programmer to implement only the
      <tt>zeroYieldImpl(Time)</tt> method in derived classes.

      Discount and forward are calculated from zero yields.

      Zero rates are assumed to be annual continuous compounding.

      \ingroup yieldtermstructures
   */
   public abstract class ZeroYieldStructure : YieldTermStructure
   {
      #region Constructors

      protected ZeroYieldStructure(DayCounter dc = null, List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(dc, jumps, jumpDates) {}

      protected ZeroYieldStructure(Date referenceDate, Calendar calendar = null, DayCounter dc = null,
                                   List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(referenceDate, calendar, dc, jumps, jumpDates) { }

      protected ZeroYieldStructure(int settlementDays, Calendar calendar, DayCounter dc = null,
                                   List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(settlementDays, calendar, dc, jumps, jumpDates) { }

      #endregion

      #region Calculations

      // This method must be implemented in derived classes to
      // perform the actual calculations. When it is called,
      // range check has already been performed; therefore, it
      // must assume that extrapolation is required.

      //! zero-yield calculation
      protected abstract double zeroYieldImpl(double t);

      #endregion

      #region YieldTermStructure implementation

      /*! Returns the discount factor for the given date calculating it
          from the zero yield.
      */
      protected internal override double discountImpl(double t)
      {
         if (t.IsEqual(0.0))     // this acts as a safe guard in cases where
            return 1.0;   // zeroYieldImpl(0.0) would throw.

         double r = zeroYieldImpl(t);
         return Math.Exp(-r * t);
      }

      #endregion
   }
}
