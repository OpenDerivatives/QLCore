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
   //! caplet const volatility model
   public class LmConstWrapperVolatilityModel : LmVolatilityModel
   {
      public LmConstWrapperVolatilityModel(LmVolatilityModel volaModel)
         : base(volaModel.size(), 0)
      {
         volaModel_ = volaModel;
      }

      public override Vector volatility(double t, Vector x = null)
      {
         return volaModel_.volatility(t, x);
      }

      public override double volatility(int i, double t, Vector x = null)
      {
         return volaModel_.volatility(i, t, x);
      }

      public override double integratedVariance(int i, int j, double u, Vector x = null)
      {
         return volaModel_.integratedVariance(i, j, u, x);
      }

      protected LmVolatilityModel volaModel_;

      public override void generateArguments()
      {}
   }
}
