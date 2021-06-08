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
using System.Linq;

namespace QLCore
{
   /// <summary>
   /// Numerical lattice engine for swaps
   /// </summary>
   public class TreeVanillaSwapEngine : LatticeShortRateModelEngine<VanillaSwap.Arguments, VanillaSwap.Results>
   {
      private Handle<YieldTermStructure> termStructure_;

      /* Constructors
          \note the term structure is only needed when the short-rate
                model cannot provide one itself.
      */

      public TreeVanillaSwapEngine(ShortRateModel model,
                                   int timeSteps,
                                   Handle<YieldTermStructure> termStructure)
         : base(model, timeSteps)
      {
         termStructure_ = termStructure;
      }

      public TreeVanillaSwapEngine(ShortRateModel model,
                                   TimeGrid timeGrid,
                                   Handle<YieldTermStructure> termStructure)
         : base(model, timeGrid)
      {
         termStructure_ = termStructure;
      }

      public override void calculate()
      {
         if (base.model_ == null)
            throw new ArgumentException("no model specified");

         Date referenceDate;
         DayCounter dayCounter;

         ITermStructureConsistentModel tsmodel =
            (ITermStructureConsistentModel) base.model_.link;
         try
         {
            if (tsmodel != null)
            {
               referenceDate = tsmodel.termStructure().link.referenceDate();
               dayCounter = tsmodel.termStructure().link.dayCounter();
            }
            else
            {
               referenceDate = termStructure_.link.referenceDate();
               dayCounter = termStructure_.link.dayCounter();
            }
         }
         catch
         {
            referenceDate = termStructure_.link.referenceDate();
            dayCounter = termStructure_.link.dayCounter();
         }

         DiscretizedSwap swap = new DiscretizedSwap(arguments_, referenceDate, dayCounter);
         List<double> times = swap.mandatoryTimes();
         Lattice lattice;

         if (lattice_ != null)
         {
            lattice = lattice_;
         }
         else
         {
            TimeGrid timeGrid = new TimeGrid(times, times.Count, timeSteps_);
            lattice = model_.link.tree(timeGrid);
         }

         swap.initialize(lattice, times.Last());
         swap.rollback(0.0);

         results_.value = swap.presentValue();
      }
   }
}
