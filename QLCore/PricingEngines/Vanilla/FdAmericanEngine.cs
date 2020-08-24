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
   //! Finite-differences pricing engine for American one asset options
   /*! \ingroup vanillaengines

       \test
       - the correctness of the returned value is tested by reproducing results available in literature.
       - the correctness of the returned greeks is tested by reproducing numerical derivatives.
   */
   public class FDAmericanEngine : FDEngineAdapter<FDAmericanCondition<FDStepConditionEngine>, OneAssetOption.Engine>,
      IFDEngine
   {
      // required for generics
      public FDAmericanEngine() { }

      public FDAmericanEngine(GeneralizedBlackScholesProcess process, int timeSteps = 100, int gridPoints = 100,
                              bool timeDependent = false)
         : base(process, timeSteps, gridPoints, timeDependent) { }

      public IFDEngine factory(GeneralizedBlackScholesProcess process, int timeSteps = 100, int gridPoints = 100)
      {
         return new FDAmericanEngine(process, timeSteps, gridPoints);
      }
   }
}
