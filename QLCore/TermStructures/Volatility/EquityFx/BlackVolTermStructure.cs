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

namespace QLCore
{
   //! Black-volatility term structure
   /*! This abstract class defines the interface of concrete
      Black-volatility term structures which will be derived from
      this one.

      Volatilities are assumed to be expressed on an annual basis.
   */
   public abstract class BlackVolTermStructure : VolatilityTermStructure
   {
      #region Constructors
      //! default constructor
      /*! \warning term structures initialized by means of this
                   constructor must manage their own reference date
                   by overriding the referenceDate() method.
      */

      protected BlackVolTermStructure(Settings settings, BusinessDayConvention bdc = BusinessDayConvention.Following, DayCounter dc = null)
         : base(settings, bdc, dc)
      {}

      //! initialize with a fixed reference date
      protected BlackVolTermStructure(Settings settings, Date referenceDate, Calendar cal = null,
                                      BusinessDayConvention bdc = BusinessDayConvention.Following, DayCounter dc = null)
         : base(settings, referenceDate, cal, bdc, dc)
      {}

      //! calculate the reference date based on the global evaluation date
      protected BlackVolTermStructure(Settings settings, int settlementDays, Calendar cal, BusinessDayConvention bdc = BusinessDayConvention.Following,
                                      DayCounter dc = null)
         : base(settings, settlementDays, cal, bdc, dc)
      {}

      #endregion

      #region Black Volatility

      //! spot volatility
      public double blackVol(Date maturity, double strike, bool extrapolate = false)
      {
         checkRange(maturity, extrapolate);
         checkStrike(strike, extrapolate);
         double t = timeFromReference(maturity);
         return blackVolImpl(t, strike);
      }

      //! spot volatility
      public double blackVol(double maturity, double strike, bool extrapolate = false)
      {
         checkRange(maturity, extrapolate);
         checkStrike(strike, extrapolate);
         return blackVolImpl(maturity, strike);
      }

      //! spot variance
      public double blackVariance(Date maturity, double strike, bool extrapolate = false)
      {
         checkRange(maturity, extrapolate);
         checkStrike(strike, extrapolate);
         double t = timeFromReference(maturity);
         return blackVarianceImpl(t, strike);
      }

      //! spot variance
      public double blackVariance(double maturity, double strike, bool extrapolate = false)
      {
         checkRange(maturity, extrapolate);
         checkStrike(strike, extrapolate);
         return blackVarianceImpl(maturity, strike);
      }

      //! forward (at-the-money) volatility
      public double blackForwardVol(Date date1, Date date2, double strike, bool extrapolate = false)
      {
         // (redundant) date-based checks
         Utils.QL_REQUIRE(date1 <= date2, () => date1 + " later than " + date2);
         checkRange(date2, extrapolate);

         // using the time implementation
         double time1 = timeFromReference(date1);
         double time2 = timeFromReference(date2);
         return blackForwardVol(time1, time2, strike, extrapolate);
      }

      //! forward (at-the-money) volatility
      public double blackForwardVol(double time1, double time2, double strike, bool extrapolate = false)
      {
         Utils.QL_REQUIRE(time1 <= time2, () => time1 + " later than " + time2);
         checkRange(time2, extrapolate);
         checkStrike(strike, extrapolate);
         if (time2.IsEqual(time1))
         {
            if (time1.IsEqual(0.0))
            {
               double epsilon = 1.0e-5;
               double var = blackVarianceImpl(epsilon, strike);
               return Math.Sqrt(var / epsilon);
            }
            else
            {
               double epsilon = Math.Min(1.0e-5, time1);
               double var1 = blackVarianceImpl(time1 - epsilon, strike);
               double var2 = blackVarianceImpl(time1 + epsilon, strike);
               Utils.QL_REQUIRE(var2 >= var1, () => "variances must be non-decreasing");
               return Math.Sqrt((var2 - var1) / (2 * epsilon));
            }
         }
         else
         {
            double var1 = blackVarianceImpl(time1, strike);
            double var2 = blackVarianceImpl(time2, strike);
            Utils.QL_REQUIRE(var2 >= var1, () => "variances must be non-decreasing");
            return Math.Sqrt((var2 - var1) / (time2 - time1));
         }
      }

      //! forward (at-the-money) variance
      public double blackForwardVariance(Date date1, Date date2,  double strike, bool extrapolate = false)
      {
         // (redundant) date-based checks
         Utils.QL_REQUIRE(date1 <= date2, () => date1 + " later than " + date2);
         checkRange(date2, extrapolate);

         // using the time implementation
         double time1 = timeFromReference(date1);
         double time2 = timeFromReference(date2);
         return blackForwardVariance(time1, time2, strike, extrapolate);
      }

      //! forward (at-the-money) variance
      public double blackForwardVariance(double time1, double time2,  double strike, bool extrapolate = false)
      {
         Utils.QL_REQUIRE(time1 <= time2, () => time1 + " later than " + time2);
         checkRange(time2, extrapolate);
         checkStrike(strike, extrapolate);
         double v1 = blackVarianceImpl(time1, strike);
         double v2 = blackVarianceImpl(time2, strike);
         Utils.QL_REQUIRE(v2 >= v1, () => "variances must be non-decreasing");
         return v2 - v1;
      }

      #endregion

      #region Calculations

      //   These methods must be implemented in derived classes to perform
      //   the actual volatility calculations. When they are called,
      //   range check has already been performed; therefore, they must
      //   assume that extrapolation is required.

      //! Black variance calculation
      protected abstract double blackVarianceImpl(double t, double strike);

      //! Black volatility calculation
      protected abstract double blackVolImpl(double t, double strike);

      #endregion

   }

   //! Black-volatility term structure
   /*! This abstract class acts as an adapter to BlackVolTermStructure
       allowing the programmer to implement only the
       <tt>blackVolImpl(Time, Real, bool)</tt> method in derived classes.

       Volatility are assumed to be expressed on an annual basis.
   */

   public abstract class BlackVolatilityTermStructure : BlackVolTermStructure
   {
      #region Constructors

      //! default constructor
      /*! \warning term structures initialized by means of this
                   constructor must manage their own reference date
                   by overriding the referenceDate() method.
      */

      protected BlackVolatilityTermStructure(Settings settings, BusinessDayConvention bdc = BusinessDayConvention.Following,
                                             DayCounter dc = null)
         : base(settings, bdc, dc)
      {}

      //! initialize with a fixed reference date
      protected BlackVolatilityTermStructure(Settings settings, Date referenceDate, Calendar cal = null,
                                             BusinessDayConvention bdc = BusinessDayConvention.Following, DayCounter dc = null)
         : base(settings, referenceDate, cal, bdc, dc)
      {}

      //! calculate the reference date based on the global evaluation date
      protected BlackVolatilityTermStructure(Settings settings, int settlementDays, Calendar cal,
                                             BusinessDayConvention bdc = BusinessDayConvention.Following, DayCounter dc = null)
         : base(settings, settlementDays, cal, bdc, dc)
      {}

      #endregion

      /*! Returns the variance for the given strike and date calculating it
          from the volatility.
      */
      protected override double blackVarianceImpl(double maturity, double strike)
      {
         double vol = blackVolImpl(maturity, strike);
         return vol * vol * maturity;
      }
   }


   //! Black variance term structure
   /*! This abstract class acts as an adapter to VolTermStructure allowing
       the programmer to implement only the
       <tt>blackVarianceImpl(Time, Real, bool)</tt> method in derived
       classes.

       Volatility are assumed to be expressed on an annual basis.
   */

   public abstract class BlackVarianceTermStructure : BlackVolTermStructure
   {
      #region Constructors
      //! default constructor
      /*! \warning term structures initialized by means of this
                   constructor must manage their own reference date
                   by overriding the referenceDate() method.
      */

      protected BlackVarianceTermStructure(Settings settings, BusinessDayConvention bdc = BusinessDayConvention.Following,
                                           DayCounter dc = null)
         : base(settings, bdc, dc)
      {}

      //! initialize with a fixed reference date
      protected BlackVarianceTermStructure(Settings settings, Date referenceDate, Calendar cal = null,
                                           BusinessDayConvention bdc = BusinessDayConvention.Following, DayCounter dc = null)
         : base(settings, referenceDate, cal, bdc, dc)
      {}

      //! calculate the reference date based on the global evaluation date
      protected BlackVarianceTermStructure(Settings settings, int settlementDays, Calendar cal,
                                           BusinessDayConvention bdc = BusinessDayConvention.Following, DayCounter dc = null)
         : base(settings, settlementDays, cal, bdc, dc)
      {}

      #endregion

      /*! Returns the volatility for the given strike and date calculating it
          from the variance.
      */
      protected override double blackVolImpl(double t, double strike)
      {
         double nonZeroMaturity = t.IsEqual(0.0) ? 0.00001 : t;
         double var = blackVarianceImpl(nonZeroMaturity, strike);
         return Math.Sqrt(var / nonZeroMaturity);
      }

   }

}
