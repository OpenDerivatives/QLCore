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
   //! Hazard-rate term structure
   /*! This abstract class acts as an adapter to
      DefaultProbabilityTermStructure allowing the programmer to implement
      only the <tt>hazardRateImpl(Time)</tt> method in derived classes.

      Survival/default probabilities and default densities are calculated
      from hazard rates.

      Hazard rates are defined with annual frequency and continuous
      compounding.

      \ingroup defaultprobabilitytermstructures
   */
   public abstract class HazardRateStructure : DefaultProbabilityTermStructure
   {
      #region Constructors

      protected HazardRateStructure(Settings settings, DayCounter dc = null, List<Handle<Quote> > jumps = null, List<Date> jumpDates = null)
         : base(settings, dc, jumps, jumpDates) {}

      protected HazardRateStructure(Settings settings, Date referenceDate, Calendar cal = null, DayCounter dc = null,
                                    List<Handle<Quote> > jumps = null, List<Date> jumpDates = null)
         : base(settings, referenceDate, cal, dc, jumps, jumpDates) { }

      protected HazardRateStructure(Settings settings, int settlementDays, Calendar cal, DayCounter dc = null,
                                    List<Handle<Quote> > jumps = null, List<Date> jumpDates = null)
         : base(settings, settlementDays, cal, dc, jumps, jumpDates) { }

      #endregion

      #region Calculations

      // This method must be implemented in derived classes to
      // perform the actual calculations. When it is called,
      // range check has already been performed; therefore, it
      // must assume that extrapolation is required.
      #endregion

      //! hazard rate calculation

      #region DefaultProbabilityTermStructure implementation

      /*! survival probability calculation
         implemented in terms of the hazard rate \f$ h(t) \f$ as
         \f[
         S(t) = \exp\left( - \int_0^t h(\tau) d\tau \right).
         \f]

         \warning This default implementation uses numerical integration,
                  which might be inefficient and inaccurate.
                  Derived classes should override it if a more efficient
                  implementation is available.
      */
      protected internal override double survivalProbabilityImpl(double t)
      {
         GaussChebyshevIntegration integral = new GaussChebyshevIntegration(48);
         // this stores the address of the method to integrate (so that
         // we don't have to insert its full expression inside the
         // integral below--it's long enough already)

         // the Gauss-Chebyshev quadratures integrate over [-1,1],
         // hence the remapping (and the Jacobian term t/2)
         return Math.Exp(-integral.value(hazardRateImpl) * t / 2.0);
      }

      //! default density calculation
      protected internal override double defaultDensityImpl(double t)
      {
         return hazardRateImpl(t) * survivalProbabilityImpl(t);
      }

      #endregion
   }
}
