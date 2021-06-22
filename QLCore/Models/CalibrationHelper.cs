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
   //! liquid market instrument used during calibration
   public abstract class CalibrationHelper : LazyObject
   {
      public enum CalibrationErrorType
      {
         RelativePriceError, PriceError, ImpliedVolError
      }

      protected CalibrationHelper(Handle<Quote> volatility,
                                  Handle<YieldTermStructure> termStructure,
                                  Settings settings,
                                  CalibrationErrorType calibrationErrorType = CalibrationErrorType.RelativePriceError,
                                  VolatilityType type = VolatilityType.ShiftedLognormal,
                                  double shift = 0.0)
      {
         volatility_ = volatility;
         termStructure_ = termStructure;
         calibrationErrorType_ = calibrationErrorType;
         volatilityType_ = type;
         shift_ = shift;
         settings_ = settings;
      }

      protected override void performCalculations()
      {
         marketValue_ = blackPrice(volatility_.link.value());
      }

      //! returns the volatility Handle
      public Handle<Quote> volatility() { return volatility_; }

      //! returns the volatility type
      public VolatilityType volatilityType() { return volatilityType_; }

      //! returns the actual price of the instrument (from volatility)
      public double marketValue() { calculate(); return marketValue_; }

      public Settings settings() { return settings_; }
      public void setSettings(Settings s) { settings_ = s; }

      //! returns the price of the instrument according to the model
      public abstract double modelValue();

      //! returns the error resulting from the model valuation
      public virtual double calibrationError()
      {
         double error = 0 ;

         switch (calibrationErrorType_)
         {
            case CalibrationErrorType.RelativePriceError:
               error = Math.Abs(marketValue() - modelValue()) / marketValue();
               break;
            case CalibrationErrorType.PriceError:
               error = marketValue() - modelValue();
               break;
            case CalibrationErrorType.ImpliedVolError:
            {
               double minVol = volatilityType_ == VolatilityType.ShiftedLognormal ? 0.0010 : 0.00005;
               double maxVol = volatilityType_ == VolatilityType.ShiftedLognormal ? 10.0 : 0.50;
               double lowerPrice = blackPrice(minVol);
               double upperPrice = blackPrice(maxVol);
               double modelPrice = modelValue();

               double implied;
               if (modelPrice <= lowerPrice)
                  implied = 0.001;
               else if (modelPrice >= upperPrice)
                  implied = 10.0;
               else
                  implied = this.impliedVolatility(modelPrice, 1e-12, 5000, 0.001, 10);
               error = implied - volatility_.link.value();
            }
            break;
            default:
               Utils.QL_FAIL("unknown Calibration Error Type");
               break;
         }

         return error;

      }

      public abstract void addTimesTo(List<double> times);

      //! Black volatility implied by the model
      public double impliedVolatility(double targetValue,
                                      double accuracy, int maxEvaluations, double minVol, double maxVol)
      {

         ImpliedVolatilityHelper f = new ImpliedVolatilityHelper(this, targetValue);
         Brent solver = new Brent();
         solver.setMaxEvaluations(maxEvaluations);
         return solver.solve(f, accuracy, volatility_.link.value(), minVol, maxVol);
      }

      //! Black price given a volatility
      public abstract double blackPrice(double volatility);

      public void setPricingEngine(IPricingEngine engine) {engine_ = engine;}


      protected double marketValue_;
      protected Handle<Quote> volatility_;
      protected Handle<YieldTermStructure> termStructure_;
      protected IPricingEngine engine_;
      protected VolatilityType volatilityType_;
      protected double shift_;
      protected Settings settings_;


      private CalibrationErrorType calibrationErrorType_;

      private class ImpliedVolatilityHelper : ISolver1d
      {
         private CalibrationHelper helper_;
         private double value_;

         public ImpliedVolatilityHelper(CalibrationHelper helper, double value)
         {
            helper_ = helper;
            value_ = value;
         }

         public override double value(double x)
         {
            return value_ - helper_.blackPrice(x);
         }
      }

   }
}
