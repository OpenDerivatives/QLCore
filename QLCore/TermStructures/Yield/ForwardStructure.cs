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

using System;
using System.Collections.Generic;

namespace QLCore
{
   //! %Forward-rate term structure
   /*! This abstract class acts as an adapter to YieldTermStructure allowing
       the programmer to implement only the <tt>forwardImpl(Time)</tt> method
       in derived classes.

       Zero yields and discounts are calculated from forwards.
       Forward rates are assumed to be annual continuous compounding.

       \ingroup yieldtermstructures
   */
   public abstract class ForwardRateStructure : YieldTermStructure
   {
      #region Constructors

      protected ForwardRateStructure(Settings settings, DayCounter dc = null,
                                     List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(settings, dc, jumps, jumpDates) {}

      protected ForwardRateStructure(Settings settings, Date refDate, Calendar cal = null, DayCounter dc = null,
                                     List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(settings, refDate, cal, dc, jumps, jumpDates) {}

      protected ForwardRateStructure(Settings settings, int settlDays, Calendar cal, DayCounter dc = null,
                                     List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(settings, settlDays, cal, dc, jumps, jumpDates) {}

      #endregion

      #region Calculations

      // These methods must be implemented in derived classes to
      // perform the actual calculations. When they are called,
      // range check has already been performed; therefore, they
      // must assume that extrapolation is required.

      //! instantaneous forward-rate calculation
      protected abstract double forwardImpl(double s);
      /*! Returns the zero yield rate for the given date calculating it
          from the instantaneous forward rate \f$ f(t) \f$ as
          \f[
          z(t) = \int_0^t f(\tau) d\tau
          \f]

          \warning This default implementation uses an highly inefficient
                   and possibly wildly inaccurate numerical integration.
                   Derived classes should override it if a more efficient
                   implementation is available.
      */
      protected virtual double zeroYieldImpl(double t)
      {
         if (t.IsEqual(0.0))
            return forwardImpl(0.0);
         // implement smarter integration if plan to use the following code
         double sum = 0.5 * forwardImpl(0.0);
         int N = 1000;
         double dt = t / N;
         for (double i = dt; i < t; i += dt)
            sum += forwardImpl(i);
         sum += 0.5 * forwardImpl(t);
         return (sum * dt / t);
      }

      #endregion

      #region YieldTermStructure implementation
      /*! Returns the discount factor for the given date calculating it
          from the zero rate as \f$ d(t) = \exp \left( -z(t) t \right) \f$
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
