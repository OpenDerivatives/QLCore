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

using System.Collections.Generic;

namespace QLCore
{
   public class CapletVarianceCurve :  OptionletVolatilityStructure
   {

      private BlackVarianceCurve blackCurve_;

      public CapletVarianceCurve(Date referenceDate,
                                 List<Date> dates,
                                 List<double> capletVolCurve,
                                 DayCounter dayCounter)
         : base(referenceDate, new Calendar(), BusinessDayConvention.Following, new DayCounter())
      {
         blackCurve_ = new BlackVarianceCurve(referenceDate, dates, capletVolCurve, dayCounter, false);
      }

      public override DayCounter dayCounter()
      {
         return blackCurve_.dayCounter();
      }

      public override Date maxDate()
      {
         return blackCurve_.maxDate();
      }

      public override double minStrike()
      {
         return blackCurve_.minStrike();
      }

      public override double  maxStrike()
      {
         return blackCurve_.maxStrike();
      }

      protected override SmileSection smileSectionImpl(double t)
      {
         // dummy strike
         double atmVol = blackCurve_.blackVol(t, 0.05, true);
         return new FlatSmileSection(t, atmVol, dayCounter());
      }

      protected override double volatilityImpl(double t, double r)
      {
         return blackCurve_.blackVol(t, r, true);
      }
   }
}
