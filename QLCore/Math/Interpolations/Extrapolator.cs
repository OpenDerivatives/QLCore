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
   //! base class for classes possibly allowing extrapolation
   // LazyObject should not be here but it is because of the InterpolatedYieldCurve
   public abstract class Extrapolator : LazyObject
   {
      private bool extrapolate_;
      public bool extrapolate { get { return extrapolate_; } set { extrapolate_ = value; } }

      // some extra functionality
      public bool allowsExtrapolation() { return extrapolate_; }      //! tells whether extrapolation is enabled
      public void enableExtrapolation(bool b = true) { extrapolate_ = b; }      //! enable extrapolation in subsequent calls
      public void disableExtrapolation(bool b = true) { extrapolate_ = !b; }    //! disable extrapolation in subsequent calls
   }
}
