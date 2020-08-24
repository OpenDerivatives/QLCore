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

using System.Collections.Generic;

// Numerical lattice engines for callable/puttable bonds
namespace QLCore
{
   //! Numerical lattice engine for callable fixed rate bonds
   /*! \ingroup callablebondengines */
   public class TreeCallableFixedRateBondEngine : LatticeShortRateModelEngine<CallableBond.Arguments, CallableBond.Results>
   {
      /* Constructors
          \note the term structure is only needed when the short-rate
                model cannot provide one itself.
      */
      public TreeCallableFixedRateBondEngine(ShortRateModel model, int  timeSteps,
                                             Handle<YieldTermStructure> termStructure)
         : base(model, timeSteps)

      {
         termStructure_ = termStructure;
         termStructure_.registerWith(update);
      }


      public TreeCallableFixedRateBondEngine(ShortRateModel model, TimeGrid timeGrid,
                                             Handle<YieldTermStructure> termStructure)
         : base(model, timeGrid)
      {
         termStructure_ = termStructure;
         termStructure_.registerWith(update);
      }

      public override void calculate()
      {
         Utils.QL_REQUIRE(model_ != null, () => "no model specified");

         Date referenceDate;
         DayCounter dayCounter;

         ITermStructureConsistentModel tsmodel = (ITermStructureConsistentModel)base.model_.link;
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

         DiscretizedCallableFixedRateBond callableBond = new DiscretizedCallableFixedRateBond(arguments_, referenceDate, dayCounter);
         Lattice lattice;

         if (lattice_ != null)
         {
            lattice = lattice_;
         }
         else
         {
            List<double> times = callableBond.mandatoryTimes();
            TimeGrid timeGrid = new TimeGrid(times, times.Count, timeSteps_);
            lattice = model_.link.tree(timeGrid);
         }

         double redemptionTime = dayCounter.yearFraction(referenceDate, arguments_.redemptionDate);
         callableBond.initialize(lattice, redemptionTime);
         callableBond.rollback(0.0);
         results_.value = results_.settlementValue = callableBond.presentValue();
      }

      private Handle<YieldTermStructure> termStructure_;
   }
}
