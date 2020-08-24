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
   public class FdmQuantoHelper : IObservable
   {
      public FdmQuantoHelper(
         YieldTermStructure rTS,
         YieldTermStructure fTS,
         BlackVolTermStructure fxVolTS,
         double equityFxCorrelation,
         double exchRateATMlevel)
      {
         rTS_ = rTS;
         fTS_ = fTS;
         fxVolTS_ = fxVolTS;
         equityFxCorrelation_ = equityFxCorrelation;
         exchRateATMlevel_ = exchRateATMlevel;
      }

      public double quantoAdjustment(double equityVol, double t1, double t2)
      {
         double rDomestic = rTS_.forwardRate(t1, t2, Compounding.Continuous).rate();
         double rForeign = fTS_.forwardRate(t1, t2, Compounding.Continuous).rate();
         double fxVol = fxVolTS_.blackForwardVol(t1, t2, exchRateATMlevel_);

         return rDomestic - rForeign + equityVol * fxVol * equityFxCorrelation_;
      }

      public Vector quantoAdjustment(Vector equityVol, double t1, double t2)
      {
         double rDomestic = rTS_.forwardRate(t1, t2, Compounding.Continuous).rate();
         double rForeign = fTS_.forwardRate(t1, t2, Compounding.Continuous).rate();
         double fxVol = fxVolTS_.blackForwardVol(t1, t2, exchRateATMlevel_);

         Vector retVal = new Vector(equityVol.size());
         for (int i = 0; i < retVal.size(); ++i)
         {
            retVal[i] = rDomestic - rForeign + equityVol[i] * fxVol * equityFxCorrelation_;
         }
         return retVal;
      }

      public YieldTermStructure foreignTermStructure() { return fTS_; }
      public YieldTermStructure riskFreeTermStructure() { return rTS_; }
      public BlackVolTermStructure fxVolatilityTermStructure() { return fxVolTS_; }
      public double equityFxCorrelation() { return equityFxCorrelation_; }
      public double exchRateATMlevel() { return exchRateATMlevel_; }

      protected YieldTermStructure rTS_, fTS_;
      protected BlackVolTermStructure fxVolTS_;
      protected double equityFxCorrelation_, exchRateATMlevel_;

      #region Observer & Observable

      // observable interface
      private readonly WeakEventSource eventSource = new WeakEventSource();

      public event Callback notifyObserversEvent
      {
         add
         {
            eventSource.Subscribe(value);
         }
         remove
         {
            eventSource.Unsubscribe(value);
         }
      }

      public void registerWith(Callback handler)
      {
         notifyObserversEvent += handler;
      }

      public void unregisterWith(Callback handler)
      {
         notifyObserversEvent -= handler;
      }

      protected void notifyObservers()
      {
         eventSource.Raise();
      }

      public virtual void update()
      {
         notifyObservers();
      }

      #endregion
   }
}
