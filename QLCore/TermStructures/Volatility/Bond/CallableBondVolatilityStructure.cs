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

using System.Collections.Generic;

namespace QLCore
{
   //! Callable-bond volatility structure
   /*! This class is purely abstract and defines the interface of
       concrete callable-bond volatility structures which will be
       derived from this one.
   */
   public abstract class CallableBondVolatilityStructure : TermStructure
   {
      //! default constructor
      /*! \warning term structures initialized by means of this
                  constructor must manage their own reference date
                  by overriding the referenceDate() method.
      */

      protected CallableBondVolatilityStructure(Settings settings, DayCounter dc = null, BusinessDayConvention bdc = BusinessDayConvention.Following)
         : base(settings, dc ?? new DayCounter())
      {
         bdc_ = bdc;
      }
      //! initialize with a fixed reference date
      protected CallableBondVolatilityStructure(Settings settings, Date referenceDate, Calendar calendar = null, DayCounter dc = null,
                                                BusinessDayConvention bdc = BusinessDayConvention.Following)
         : base(settings, referenceDate, calendar ?? new Calendar(), dc ?? new DayCounter())
      {
         bdc_ = bdc;
      }
      //! calculate the reference date based on the global evaluation date
      protected CallableBondVolatilityStructure(Settings settings, int settlementDays, Calendar calendar, DayCounter dc = null,
                                                BusinessDayConvention bdc = BusinessDayConvention.Following)
         : base(settings, settlementDays, calendar, dc ?? new DayCounter())
      {
         bdc_ = bdc;
      }
      //! returns the volatility for a given option time and bondLength
      public double volatility(double optionTenor, double bondTenor, double strike, bool extrapolate = false)
      {
         checkRange(optionTenor, bondTenor, strike, extrapolate);
         return volatilityImpl(optionTenor, bondTenor, strike);
      }
      //! returns the Black variance for a given option time and bondLength
      public double blackVariance(double optionTime, double bondLength, double strike, bool extrapolate = false)
      {
         checkRange(optionTime, bondLength, strike, extrapolate);
         double vol = volatilityImpl(optionTime, bondLength, strike);
         return vol * vol * optionTime;
      }
      //! returns the volatility for a given option date and bond tenor
      public double volatility(Date optionDate, Period bondTenor, double strike, bool extrapolate = false)
      {
         checkRange(optionDate, bondTenor, strike, extrapolate);
         return volatilityImpl(optionDate, bondTenor, strike);
      }
      //! returns the Black variance for a given option date and bond tenor
      public double blackVariance(Date optionDate, Period bondTenor, double strike, bool extrapolate = false)
      {
         double vol =  volatility(optionDate, bondTenor, strike, extrapolate);
         KeyValuePair<double, double> p = convertDates(optionDate, bondTenor);
         return vol * vol * p.Key;
      }
      public virtual SmileSection smileSection(Date optionDate, Period bondTenor)
      {
         KeyValuePair<double, double> p = convertDates(optionDate, bondTenor);
         return smileSectionImpl(p.Key, p.Value);
      }

      //! returns the volatility for a given option tenor and bond tenor
      public double volatility(Period optionTenor, Period bondTenor, double strike, bool extrapolate = false)
      {
         Date optionDate = optionDateFromTenor(optionTenor);
         return volatility(optionDate, bondTenor, strike, extrapolate);
      }
      //! returns the Black variance for a given option tenor and bond tenor
      public double blackVariance(Period optionTenor, Period bondTenor, double strike, bool extrapolate = false)
      {
         Date optionDate = optionDateFromTenor(optionTenor);
         double vol = volatility(optionDate, bondTenor, strike, extrapolate);
         KeyValuePair<double, double> p = convertDates(optionDate, bondTenor);
         return vol * vol * p.Key;
      }
      public SmileSection smileSection(Period optionTenor, Period bondTenor)
      {
         Date optionDate = optionDateFromTenor(optionTenor);
         return smileSection(optionDate, bondTenor);
      }
      // Limits
      //! the largest length for which the term structure can return vols
      public abstract Period maxBondTenor();
      //! the largest bondLength for which the term structure can return vols
      public virtual double maxBondLength()
      {
         return timeFromReference(referenceDate() + maxBondTenor());
      }
      //! the minimum strike for which the term structure can return vols
      public abstract double minStrike();
      //! the maximum strike for which the term structure can return vols
      public abstract double maxStrike();

      //! implements the conversion between dates and times
      public virtual KeyValuePair<double, double> convertDates(Date optionDate, Period bondTenor)
      {
         Date end = optionDate + bondTenor;
         Utils.QL_REQUIRE(end > optionDate, () =>
                          "negative bond tenor (" + bondTenor + ") given");
         double optionTime = timeFromReference(optionDate);
         double timeLength = dayCounter().yearFraction(optionDate, end);
         return new KeyValuePair<double, double>(optionTime, timeLength);
      }
      //! the business day convention used for option date calculation
      public virtual BusinessDayConvention businessDayConvention() { return bdc_; }
      //! implements the conversion between optionTenors and optionDates
      public Date optionDateFromTenor(Period optionTenor)
      {
         return calendar().advance(referenceDate(),
                                   optionTenor,
                                   businessDayConvention());
      }

      //! return smile section
      protected abstract SmileSection smileSectionImpl(double optionTime, double bondLength);

      //! implements the actual volatility calculation in derived classes
      protected abstract double volatilityImpl(double optionTime, double bondLength, double strike);
      protected virtual double volatilityImpl(Date optionDate, Period bondTenor, double strike)
      {
         KeyValuePair<double, double> p = convertDates(optionDate, bondTenor);
         return volatilityImpl(p.Key, p.Value, strike);
      }
      protected void checkRange(double optionTime, double bondLength, double k, bool extrapolate)
      {
         base.checkRange(optionTime, extrapolate);
         Utils.QL_REQUIRE(bondLength >= 0.0, () =>
                          "negative bondLength (" + bondLength + ") given");
         Utils.QL_REQUIRE(extrapolate || allowsExtrapolation() ||
                          bondLength <= maxBondLength(), () =>
                          "bondLength (" + bondLength + ") is past max curve bondLength ("
                          + maxBondLength() + ")");
         Utils.QL_REQUIRE(extrapolate || allowsExtrapolation() ||
                          (k >= minStrike() && k <= maxStrike()), () =>
                          "strike (" + k + ") is outside the curve domain ["
                          + minStrike() + "," + maxStrike() + "]");
      }
      protected void checkRange(Date optionDate, Period bondTenor, double k, bool extrapolate)
      {
         base.checkRange(timeFromReference(optionDate),
                         extrapolate);
         Utils.QL_REQUIRE(bondTenor.length() > 0, () =>
                          "negative bond tenor (" + bondTenor + ") given");
         Utils.QL_REQUIRE(extrapolate || allowsExtrapolation() ||
                          bondTenor <= maxBondTenor(), () =>
                          "bond tenor (" + bondTenor + ") is past max tenor ("
                          + maxBondTenor() + ")");
         Utils.QL_REQUIRE(extrapolate || allowsExtrapolation() ||
                          (k >= minStrike() && k <= maxStrike()), () =>
                          "strike (" + k + ") is outside the curve domain ["
                          + minStrike() + "," + maxStrike() + "]");
      }

      private BusinessDayConvention bdc_;
   }
}
