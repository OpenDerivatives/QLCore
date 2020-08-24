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
   public class PSACurve : IPrepayModel
   {

      public PSACurve(Date startdate)
         : this(startdate, 1) {}

      public PSACurve(Date startdate, double multiplier)
      {
         _startDate = startdate;
         _multi = multiplier;
      }

      public double getCPR(Date valDate)
      {
         Thirty360 dayCounter = new Thirty360();
         int d = dayCounter.dayCount(_startDate, valDate) / 30 + 1;

         return (d <= 30 ? 0.06 * (d / 30d) : 0.06) * _multi;
      }

      public double getSMM(Date valDate)
      {
         return 1 - Math.Pow((1 - getCPR(valDate)), (1 / 12d));
      }

      private Date _startDate;
      private double _multi;
   }
}
