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
   /// <summary>
   /// wrapper around Dupire local volatility surface,
   /// which does not throw exception if local volatility becomes negative
   /// </summary>
   public class NoExceptLocalVolSurface : LocalVolSurface
   {
      public NoExceptLocalVolSurface(Handle<BlackVolTermStructure> blackTS,
                                     Handle<YieldTermStructure> riskFreeTS,
                                     Handle<YieldTermStructure> dividendTS,
                                     Handle<Quote> underlying,
                                     double illegalLocalVolOverwrite)
         : base(blackTS, riskFreeTS, dividendTS, underlying)
      {
         illegalLocalVolOverwrite_ = illegalLocalVolOverwrite;
      }

      public NoExceptLocalVolSurface(Handle<BlackVolTermStructure> blackTS,
                                     Handle<YieldTermStructure> riskFreeTS,
                                     Handle<YieldTermStructure> dividendTS,
                                     double underlying,
                                     double illegalLocalVolOverwrite)
         : base(blackTS, riskFreeTS, dividendTS, underlying)
      {
         illegalLocalVolOverwrite_ = illegalLocalVolOverwrite;
      }

      protected override double localVolImpl(double t, double underlyingLevel)
      {
         double vol;
         try
         {
            vol = base.localVolImpl(t, underlyingLevel);
         }
         catch
         {
            vol = illegalLocalVolOverwrite_;
         }

         return vol;
      }

      protected double illegalLocalVolOverwrite_;
   }
}
