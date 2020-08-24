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
   /// <summary>
   /// Craig-Sneyd operator splitting
   /// </summary>
   public class CraigSneydScheme : IMixedScheme, ISchemeFactory
   {
      public CraigSneydScheme()
      { }

      public CraigSneydScheme(double theta, double mu,
                              FdmLinearOpComposite map,
                              List<BoundaryCondition<FdmLinearOp>> bcSet = null)
      {
         dt_ = null;
         theta_ = theta;
         mu_ = mu;
         map_ = map;
         bcSet_ = new BoundaryConditionSchemeHelper(bcSet);
      }

      #region ISchemeFactory

      public IMixedScheme factory(object L, object bcs, object[] additionalInputs = null)
      {
         double? theta = additionalInputs[0] as double?;
         double? mu = additionalInputs[1] as double?;
         return new CraigSneydScheme(theta.Value, mu.Value,
                                     L as FdmLinearOpComposite, bcs as List<BoundaryCondition<FdmLinearOp>>);
      }

      #endregion

      #region IMixedScheme interface

      public void step(ref object a, double t, double theta = 1.0)
      {
         Utils.QL_REQUIRE(t - dt_.Value > -1e-8, () => "a step towards negative time given");

         map_.setTime(Math.Max(0.0, t - dt_.Value), t);
         bcSet_.setTime(Math.Max(0.0, t - dt_.Value));

         bcSet_.applyBeforeApplying(map_);
         Vector y = (a as Vector) + dt_.Value * map_.apply(a as Vector);
         bcSet_.applyAfterApplying(y);

         Vector y0 = y;

         for (int i = 0; i < map_.size(); ++i)
         {
            Vector rhs = y - theta_ * dt_.Value * map_.apply_direction(i, a as Vector);
            y = map_.solve_splitting(i, rhs, -theta_ * dt_.Value);
         }

         bcSet_.applyBeforeApplying(map_);
         Vector yt = y0 + mu_ * dt_.Value * map_.apply_mixed(y - (a as Vector));
         bcSet_.applyAfterApplying(yt);

         for (int i = 0; i < map_.size(); ++i)
         {
            Vector rhs = yt - theta_ * dt_.Value * map_.apply_direction(i, a as Vector);
            yt = map_.solve_splitting(i, rhs, -theta_ * dt_.Value);
         }
         bcSet_.applyAfterSolving(yt);

         a = yt;
      }

      public void setStep(double dt)
      {
         dt_ = dt;
      }

      #endregion

      protected double? dt_;
      protected double theta_, mu_;
      protected FdmLinearOpComposite map_;
      protected BoundaryConditionSchemeHelper bcSet_;
   }
}
