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
   //! base class for early exercise path pricers
   // Returns the value of an option on a given path and given time.

   public static class EarlyExerciseTraits<PathType> where PathType : IPath
   {
      public static int pathLength(PathType path)
      {
         return path.length();
      }
   }

   public interface IEarlyExercisePathPricer<PathType, StateType> where PathType : IPath
   {
      double value(PathType path, int t);

      StateType state(PathType path, int t);
      List<Func<StateType, double>> basisSystem();
   }
}
