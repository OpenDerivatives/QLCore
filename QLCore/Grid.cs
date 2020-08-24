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

namespace QLCore
{
   public partial class Utils
   {

      public static Vector CenteredGrid(double center, double dx, int steps)
      {
         Vector result = new Vector(steps + 1);
         for (int i = 0; i < steps + 1; i++)
            result[i] = center + (i - steps / 2.0) * dx;
         return result;
      }

      public static Vector BoundedGrid(double xMin, double xMax, int steps)
      {
         return new Vector(steps + 1, xMin, (xMax - xMin) / steps);
      }

      public static Vector BoundedLogGrid(double xMin, double xMax, int steps)
      {
         Vector result = new Vector(steps + 1);
         double gridLogSpacing = (Math.Log(xMax) - Math.Log(xMin)) /
                                 (steps);
         double edx = Math.Exp(gridLogSpacing);
         result[0] = xMin;
         for (int j = 1; j < steps + 1; j++)
         {
            result[j] = result[j - 1] * edx;
         }
         return result;
      }

   }
}
