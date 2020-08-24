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

using System.Collections.Generic;

namespace QLCore
{
   /// <summary>
   /// bermudan step condition for multi dimensional problems
   /// </summary>
   public class FdmBermudanStepCondition : IStepCondition<Vector>
   {
      public FdmBermudanStepCondition(List<Date> exerciseDates,
                                      Date referenceDate,
                                      DayCounter dayCounter,
                                      FdmMesher mesher,
                                      FdmInnerValueCalculator calculator)
      {
         mesher_ = mesher;
         calculator_ = calculator;

         exerciseTimes_ = new List<double>();
         foreach (Date iter in exerciseDates)
         {
            exerciseTimes_.Add(
               dayCounter.yearFraction(referenceDate, iter));
         }
      }

      public void applyTo(object o, double t)
      {
         Vector a = (Vector) o;
         if (exerciseTimes_.BinarySearch(t) >= 0)
         {
            FdmLinearOpLayout layout = mesher_.layout();
            FdmLinearOpIterator endIter = layout.end();

            int dims = layout.dim().Count;
            Vector locations = new Vector(dims);

            for (FdmLinearOpIterator iter = layout.begin();
                 iter != endIter;
                 ++iter)
            {
               for (int i = 0; i < dims; ++i)
                  locations[i] = mesher_.location(iter, i);

               double innerValue = calculator_.innerValue(iter, t);
               if (innerValue > a[iter.index()])
               {
                  a[iter.index()] = innerValue;
               }
            }
         }
      }

      public List<double> exerciseTimes()
      {
         return exerciseTimes_;
      }

      protected List<double> exerciseTimes_;
      protected FdmMesher mesher_;
      protected FdmInnerValueCalculator calculator_;
   }
}
