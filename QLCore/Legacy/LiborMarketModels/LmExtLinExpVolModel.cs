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
   //! extended linear exponential volatility model
   /*! This class describes an extended linear-exponential volatility model

       \f[
       \sigma_i(t)=k_i*((a*(T_{i}-t)+d)*e^{-b(T_{i}-t)}+c)
       \f]

       References:

       Damiano Brigo, Fabio Mercurio, Massimo Morini, 2003,
       Different Covariance Parameterizations of Libor Market Model and Joint
       Caps/Swaptions Calibration,
       (<http://www.business.uts.edu.au/qfrc/conferences/qmf2001/Brigo_D.pdf>)
   */

   public class LmExtLinearExponentialVolModel : LmLinearExponentialVolatilityModel
   {
      public LmExtLinearExponentialVolModel(List<double> fixingTimes, double a, double b, double c, double d)
         : base(fixingTimes, a, b, c, d)
      {
         arguments_.Capacity += size_;
         for (int i = 0; i < size_; ++i)
         {
            arguments_.Add(new ConstantParameter(1.0, new PositiveConstraint()));
         }
      }

      public override Vector volatility(double t, Vector x = null)
      {
         Vector tmp = base.volatility(t, x);
         for (int i = 0; i < size_; ++i)
         {
            tmp[i] *= arguments_[i + 4].value(0.0);
         }
         return tmp;
      }

      public override double volatility(int i, double t, Vector x = null)
      {
         return arguments_[i + 4].value(0.0) * base.volatility(i, t, x);
      }

      public override double integratedVariance(int i, int j, double u, Vector x = null)
      {
         return arguments_[i + 4].value(0.0) * arguments_[j + 4].value(0.0) * base.integratedVariance(i, j, u, x);
      }
   }
}
