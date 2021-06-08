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
   //! Finite-differences Bermudan engine
   /*! \ingroup vanillaengines */
   public class FDBermudanEngine : FDMultiPeriodEngine, IGenericEngine
   {
      protected double extraTermInBermudan;

      // constructor
      public FDBermudanEngine(GeneralizedBlackScholesProcess process, int timeSteps = 100, int gridPoints = 100,
                              bool timeDependent = false)
         : base(process, timeSteps, gridPoints, timeDependent) { }

      public void calculate()
      {
         setupArguments(arguments_);
         base.calculate(results_);
      }

      protected override void initializeStepCondition()
      {
         stepCondition_ = new NullCondition<Vector>();
      }

      protected override void executeIntermediateStep(int i)
      {
         int size = intrinsicValues_.size();
         for (int j = 0; j < size; j++)
            prices_.setValue(j, Math.Max(prices_.value(j), intrinsicValues_.value(j)));
      }

      #region IGenericEngine copy-cat
      protected OneAssetOption.Arguments arguments_ = new OneAssetOption.Arguments();
      protected OneAssetOption.Results results_ = new OneAssetOption.Results();

      public IPricingEngineArguments getArguments() { return arguments_; }
      public IPricingEngineResults getResults() { return results_; }
      public void reset() { results_.reset(); }

      #endregion
   }
}
