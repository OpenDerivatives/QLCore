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
   //! Optionlet (caplet/floorlet) volatility structure
   /*! This class is purely abstract and defines the interface of
      concrete structures which will be derived from this one.
   */
   public abstract class OptionletVolatilityStructure : VolatilityTermStructure
   {
      #region Constructors
      //! default constructor
      /*! \warning term structures initialized by means of this
                   constructor must manage their own reference date
                   by overriding the referenceDate() method.
      */

      protected OptionletVolatilityStructure(Settings settings, BusinessDayConvention bdc = BusinessDayConvention.Following,
                                             DayCounter dc = null)
         : base(settings, bdc, dc) {}

      //! initialize with a fixed reference date
      protected OptionletVolatilityStructure(Settings settings, Date referenceDate, Calendar cal, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, referenceDate, cal, bdc, dc) {}

      //! calculate the reference date based on the global evaluation date
      protected OptionletVolatilityStructure(Settings settings, int settlementDays, Calendar cal, BusinessDayConvention bdc, DayCounter dc = null)
         : base(settings, settlementDays, cal, bdc, dc) {}

      #endregion

      #region Volatility and Variance

      //! returns the volatility for a given option tenor and strike rate
      public double volatility(Period optionTenor, double strike, bool extrapolate = false)
      {
         Date optionDate = optionDateFromTenor(optionTenor);
         return volatility(optionDate, strike, extrapolate);
      }

      //! returns the volatility for a given option date and strike rate
      public double volatility(Date optionDate, double strike, bool extrapolate = false)
      {
         checkRange(optionDate, extrapolate);
         checkStrike(strike, extrapolate);
         return volatilityImpl(optionDate, strike);
      }

      //! returns the volatility for a given option time and strike rate
      public double volatility(double optionTime, double strike, bool extrapolate = false)
      {
         checkRange(optionTime, extrapolate);
         checkStrike(strike, extrapolate);
         return volatilityImpl(optionTime, strike);
      }

      //! returns the Black variance for a given option tenor and strike rate
      public double blackVariance(Period optionTenor, double strike, bool extrapolate = false)
      {
         Date optionDate = optionDateFromTenor(optionTenor);
         return blackVariance(optionDate, strike, extrapolate);
      }

      //! returns the Black variance for a given option date and strike rate
      public double blackVariance(Date optionDate, double strike, bool extrapolate = false)
      {
         double v = volatility(optionDate, strike, extrapolate);
         double t = timeFromReference(optionDate);
         return v * v * t;
      }

      //! returns the Black variance for a given option time and strike rate
      public double blackVariance(double optionTime,  double strike,  bool extrapolate = false)
      {
         double v = volatility(optionTime, strike, extrapolate);
         return v * v * optionTime;
      }

      //! returns the smile for a given option tenor
      public SmileSection smileSection(Period optionTenor, bool extr = false)
      {
         Date optionDate = optionDateFromTenor(optionTenor);
         return smileSection(optionDate, extrapolate);
      }

      //! returns the smile for a given option date
      public SmileSection smileSection(Date optionDate, bool extr = false)
      {
         checkRange(optionDate, extrapolate);
         return smileSectionImpl(optionDate);
      }

      //! returns the smile for a given option time
      public SmileSection smileSection(double optionTime,  bool extr = false)
      {
         checkRange(optionTime, extrapolate);
         return smileSectionImpl(optionTime);
      }

      #endregion

      public virtual double displacement() {return 0.0;}
      public virtual VolatilityType volatilityType() {return VolatilityType.ShiftedLognormal;}

      protected virtual SmileSection smileSectionImpl(Date optionDate)
      {
         return smileSectionImpl(timeFromReference(optionDate));
      }

      //! implements the actual smile calculation in derived classes
      protected abstract SmileSection smileSectionImpl(double optionTime);

      protected double volatilityImpl(Date optionDate, double strike)
      {
         return volatilityImpl(timeFromReference(optionDate), strike);
      }

      //! implements the actual volatility calculation in derived classes
      protected abstract double volatilityImpl(double optionTime, double strike);


   }

}
