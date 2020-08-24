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
   public class CouponConversion
   {
      public CouponConversion(DateTime date, double rate)
      {
         Date = date;
         Rate = rate;
      }

      public DateTime Date { get; set; }
      public double Rate { get; set; }
      public override string ToString() => ($"Conversion Date : {Date}\nConversion Rate : {Rate}");
   }

   public class CouponConversionSchedule : List<CouponConversion>
   {}

   public static partial class Utils
   {
      public static List<double> CreateCouponSchedule(Schedule schedule,
                                                      CouponConversionSchedule couponConversionSchedule)
      {
         List<double> ret = new InitializedList<double>(schedule.Count);
         for (int i = 0 ; i < couponConversionSchedule.Count; i++)
            for (int j = 0; j < schedule.Count; j++)
               if (schedule[j] >= (Date)couponConversionSchedule[i].Date)
                  ret[j] = couponConversionSchedule[i].Rate;

         return ret;
      }

   }
}
