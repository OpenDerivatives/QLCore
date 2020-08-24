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
using System.Linq;
using System.Text;

namespace QLCore
{
   public static class ListExtension
   {
      //    list: List<T> to resize
      //    size: desired new size
      //    element: default value to insert

      public static void Resize<T>(this List<T> list, int size, T element = default(T))
      {
         int count = list.Count;

         if (size < count)
         {
            list.RemoveRange(size, count - size);
         }
         else if (size > count)
         {
            if (size > list.Capacity)     // Optimization
               list.Capacity = size;

            list.AddRange(Enumerable.Repeat(element, size - count));
         }
      }

      // erases the contents without changing the size
      public static void Erase<T>(this List<T> list)
      {
         for (int i = 0; i < list.Count; i++)
            list[i] = default(T);
      }
   }
}
