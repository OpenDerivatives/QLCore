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
   //! Generates random paths using a sequence generator
   /*! Generates random paths with drift(S,t) and variance(S,t)
       using a gaussian sequence generator

       \ingroup mcarlo

       \test the generated paths are checked against cached results
   */

   public class PathGenerator<GSG> : IPathGenerator<GSG> where GSG : IRNG
   {
      private bool brownianBridge_;
      private GSG generator_;
      private int dimension_;
      private TimeGrid timeGrid_;
      private StochasticProcess1D process_;
      private Sample<IPath> next_;
      private List<double> temp_;
      private BrownianBridge bb_;

      // constructors
      public PathGenerator(StochasticProcess process, double length, int timeSteps, GSG generator, bool brownianBridge)
      {
         brownianBridge_ = brownianBridge;
         generator_ = generator;
         dimension_ = generator_.dimension();
         timeGrid_ = new TimeGrid(length, timeSteps);
         process_ = process as StochasticProcess1D;
         next_ = new Sample<IPath>(new Path(timeGrid_), 1.0);
         temp_ = new InitializedList<double>(dimension_);
         bb_ = new BrownianBridge(timeGrid_);
         Utils.QL_REQUIRE(dimension_ == timeSteps, () =>
                          "sequence generator dimensionality (" + dimension_ + ") != timeSteps (" + timeSteps + ")");
      }

      public PathGenerator(StochasticProcess process, TimeGrid timeGrid, GSG generator, bool brownianBridge)
      {
         brownianBridge_ = brownianBridge;
         generator_ = generator;
         dimension_ = generator_.dimension();
         timeGrid_ = timeGrid;
         process_ = process as StochasticProcess1D;
         next_ = new Sample<IPath>(new Path(timeGrid_), 1.0);
         temp_ = new InitializedList<double>(dimension_);
         bb_ = new BrownianBridge(timeGrid_);

         Utils.QL_REQUIRE(dimension_ == timeGrid_.size() - 1, () =>
                          "sequence generator dimensionality (" + dimension_ + ") != timeSteps (" + (timeGrid_.size() - 1) + ")");
      }

      public Sample<IPath> next()
      {
         return next(false);
      }

      public Sample<IPath> antithetic()
      {
         return next(true);
      }

      private Sample<IPath> next(bool antithetic)
      {
         Sample<List<double>> sequence_ =
            antithetic
            ? generator_.lastSequence()
            : generator_.nextSequence();

         if (brownianBridge_)
         {
            bb_.transform(sequence_.value, temp_);
         }
         else
         {
            temp_ = new List<double>(sequence_.value);
         }

         next_.weight = sequence_.weight;

         Path path = (Path) next_.value;
         path.setFront(process_.x0());

         for (int i = 1; i < path.length(); i++)
         {
            double t = timeGrid_[i - 1];
            double dt = timeGrid_.dt(i - 1);
            path[i] = process_.evolve(t, path[i - 1], dt,
                                      antithetic
                                      ? -temp_[i - 1]
                                      : temp_[i - 1]);
         }
         return next_;
      }
   }
}
