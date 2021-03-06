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
   public static class DoubleExtension
   {
      // Fix double comparison
      public static bool IsEqual(this double d1, double d2)
      {
         if (Math.Abs(d1 - d2) <= double.Epsilon)
            return true;
         return false;
      }

      public static bool IsNotEqual(this double d1, double d2)
      {
         return ! IsEqual(d1, d2);
      }

      // Fix double? comparison
      public static bool IsEqual(this double? d1, double d2)
      {
         if (!d1.HasValue)
            return false;
         return d1.Value.IsEqual(d2);
      }

      public static bool IsNotEqual(this double? d1, double d2)
      {
         if (!d1.HasValue)
            return true;
         return d1.Value.IsNotEqual(d2);
      }

      public static bool IsEqual(this double? d1, double? d2)
      {
         if (!d1.HasValue && !d2.HasValue)
            return true;
         if (!d1.HasValue)
            return false;
         if (!d2.HasValue)
            return false;

         return d1.Value.IsEqual(d2.Value);
      }

      public static bool IsNotEqual(this double? d1, double? d2)
      {
         if (!d1.HasValue && !d2.HasValue)
            return false;
         if (!d1.HasValue)
            return true;
         if (!d2.HasValue)
            return true;
         return d1.Value.IsNotEqual(d2.Value);
      }
   }
}
