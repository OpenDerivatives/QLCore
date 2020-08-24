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

namespace QLCore
{
   //! \f$ D_{+}D_{-} \f$ matricial representation
   /*! The differential operator \f$  D_{+}D_{-} \f$ discretizes the
       second derivative with the second-order formula

       \ingroup findiff

       \test the correctness of the returned values is tested by
             checking them against numerical calculations.
   */
   public class DPlusDMinus : TridiagonalOperator
   {
      public DPlusDMinus(int gridPoints, double h)
         : base(gridPoints)
      {
         setFirstRow(0.0, 0.0);                  // linear extrapolation
         setMidRows(1 / (h * h), -2 / (h * h), 1 / (h * h));
         setLastRow(0.0, 0.0);                   // linear extrapolation
      }
   }
}
