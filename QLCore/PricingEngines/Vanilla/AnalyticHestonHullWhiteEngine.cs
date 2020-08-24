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
using System.Numerics;

namespace QLCore
{
   //! Analytic Heston engine incl. stochastic interest rates
   /*!
       References:

       Karel in't Hout, Joris Bierkens, Antoine von der Ploeg,
       Joe in't Panhuis, A Semi closed-from analytic pricing formula for
       call options in a hybrid Heston-Hull-White Model.

       A. Sepp, Pricing European-Style Options under Jump Diffusion
       Processes with Stochastic Volatility: Applications of Fourier
       Transform (<http://math.ut.ee/~spartak/papers/stochjumpvols.pdf>)

       \ingroup vanillaengines

       \test the correctness of the returned value is tested by
             reproducing results available in web/literature, testing
             against QuantLib's analytic Heston and
             Black-Scholes-Merton Hull-White engine
   */
   public class AnalyticHestonHullWhiteEngine : AnalyticHestonEngine
   {

      // see AnalticHestonEninge for usage of different constructors
      public AnalyticHestonHullWhiteEngine(HestonModel hestonModel,
                                           HullWhite hullWhiteModel,
                                           int integrationOrder = 144)
         : base(hestonModel, integrationOrder)
      {
         hullWhiteModel_ = hullWhiteModel;

         update();
         hullWhiteModel_.registerWith(update);
      }

      public AnalyticHestonHullWhiteEngine(HestonModel hestonModel,
                                           HullWhite hullWhiteModel,
                                           double relTolerance, int maxEvaluations)
         : base(hestonModel, relTolerance, maxEvaluations)
      {
         hullWhiteModel_ = hullWhiteModel;

         update();
         hullWhiteModel_.registerWith(update);
      }

      public void setupArguments(VanillaOption.Arguments args)
      {
         this.arguments_ = args;
      }

      public override void update()
      {
         a_ = hullWhiteModel_.parameters()[0];
         sigma_ = hullWhiteModel_.parameters()[1];
         base.update();
      }

      public override void calculate()
      {
         double t = model_.link.process().time(arguments_.exercise.lastDate());
         if (a_ * t > Math.Pow(Const.QL_EPSILON, 0.25))
         {
            m_ = sigma_ * sigma_ / (2 * a_ * a_)
                 * (t + 2 / a_ * Math.Exp(-a_ * t) - 1 / (2 * a_) * Math.Exp(-2 * a_ * t) - 3 / (2 * a_));
         }
         else
         {
            // low-a algebraic limit
            m_ = 0.5 * sigma_ * sigma_ * t * t * t * (1 / 3.0 - 0.25 * a_ * t + 7 / 60.0 * a_ * a_ * t * t);
         }

         base.calculate();
      }

      protected override Complex addOnTerm(double u, double t, int j)
      {
         return new Complex(-m_ * u * u, u * (m_ - 2 * m_ * (j - 1)));
      }

      protected HullWhite hullWhiteModel_;

      private double m_;
      private double a_, sigma_;
   }
}
