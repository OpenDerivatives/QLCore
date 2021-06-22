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
   //! Flat hazard-rate curve
   /*! \ingroup defaultprobabilitytermstructures */
   public class FlatHazardRate : HazardRateStructure
   {
      #region Constructors

      public FlatHazardRate(Settings settings, Date referenceDate, Handle<Quote> hazardRate, DayCounter dc)
         : base(settings, referenceDate, new Calendar(), dc)
      {
         hazardRate_ = hazardRate;
      }

      public FlatHazardRate(Settings settings, Date referenceDate, double hazardRate, DayCounter dc)
         : base(settings, referenceDate, new Calendar(), dc)
      {
         hazardRate_ = new Handle<Quote>(new SimpleQuote(hazardRate));
      }

      public FlatHazardRate(Settings settings, int settlementDays, Calendar calendar, Handle<Quote> hazardRate, DayCounter dc)
         : base(settings, settlementDays, calendar, dc)
      {
         hazardRate_ = hazardRate;
      }

      public FlatHazardRate(Settings settings, int settlementDays, Calendar calendar, double hazardRate, DayCounter dc)
         : base(settings, settlementDays, calendar, dc)
      {
         hazardRate_ = new Handle<Quote>(new SimpleQuote(hazardRate));
      }

      #endregion


      #region TermStructure interface

      public override Date maxDate()  { return Date.maxDate(); }

      #endregion


      #region HazardRateStructure interface

      protected internal override double hazardRateImpl(double t) { return hazardRate_.link.value(); }

      #endregion

      #region DefaultProbabilityTermStructure interface

      protected internal override double survivalProbabilityImpl(double t)
      {
         return Math.Exp(-hazardRate_.link.value() * t);
      }

      #endregion

      private Handle<Quote> hazardRate_;
   }
}
