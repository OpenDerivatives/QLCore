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

namespace QLCore
{

   //! Engine for a short-rate model specialized on a lattice
   /*! Derived engines only need to implement the <tt>calculate()</tt>
       method
   */
   public class LatticeShortRateModelEngine<ArgumentsType, ResultsType>
      : GenericModelEngine<ShortRateModel, ArgumentsType, ResultsType>
        where ArgumentsType : IPricingEngineArguments, new ()
        where ResultsType :  IPricingEngineResults, new ()

   {
      protected TimeGrid timeGrid_;
      protected int timeSteps_;
      protected Lattice lattice_;

      public LatticeShortRateModelEngine(ShortRateModel model,
                                         int timeSteps)
         : base(model)
      {
         timeSteps_ = timeSteps;
         Utils.QL_REQUIRE(timeSteps > 0, () => "timeSteps must be positive, " + timeSteps + " not allowed");
      }

      public LatticeShortRateModelEngine(ShortRateModel model,
                                         TimeGrid timeGrid)
         : base(model)
      {
         timeGrid_ = new TimeGrid(timeGrid.Last(), timeGrid.size() - 1 /*timeGrid.dt(1) - timeGrid.dt(0)*/);
         timeGrid_ = timeGrid;
         timeSteps_ = 0;
         lattice_ = this.model_.link.tree(timeGrid);
      }

      #region PricingEngine
      #region Observer & Observable
      public override void update()
      {
         if (!timeGrid_.empty())
            lattice_ = this.model_.link.tree(timeGrid_);
         notifyObservers();
      }
      #endregion
      #endregion
   }

}