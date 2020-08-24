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

namespace QLCore
{
   // this is template version to serve as base for FDStepConditionEngine and FDMultiPeriodEngine
   public class FDConditionEngineTemplate : FDVanillaEngine
   {
      #region Common definitions for deriving classes
      protected IStepCondition<Vector> stepCondition_;
      protected SampledCurve prices_;
      protected virtual void initializeStepCondition()
      {
         if (stepConditionImpl_ == null)
            stepCondition_ = new NullCondition<Vector>();
         else
            stepCondition_ = stepConditionImpl_();
      }

      protected Func<IStepCondition<Vector>> stepConditionImpl_;
      public void setStepCondition(Func<IStepCondition<Vector>> impl)
      {
         stepConditionImpl_ = impl;
      }
      #endregion

      // required for generics
      public FDConditionEngineTemplate() { }

      public FDConditionEngineTemplate(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
         : base(process, timeSteps, gridPoints, timeDependent) { }
   }

   // this is template version to serve as base for FDAmericanCondition and FDShoutCondition
   public class FDConditionTemplate<baseEngine> : FDConditionEngineTemplate
      where baseEngine : FDConditionEngineTemplate, new ()
   {
      #region Common definitions for deriving classes
      protected baseEngine engine_;

      // below is a wrap-up of baseEngine instead of c++ template inheritance
      public override void setupArguments(IPricingEngineArguments a) { engine_.setupArguments(a); }
      public override void calculate(IPricingEngineResults r) { engine_.calculate(r); }
      #endregion

      // required for generics
      public FDConditionTemplate() { }

      public FDConditionTemplate(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
         : base(process, timeSteps, gridPoints, timeDependent)
      {
         // init engine
         engine_ = (baseEngine)FastActivator<baseEngine>.Create().factory(process, timeSteps, gridPoints, timeDependent);
      }
   }


   public class FDAmericanCondition<baseEngine> : FDConditionTemplate<baseEngine>
      where baseEngine : FDConditionEngineTemplate, new ()
   {

      // required for generics
      public FDAmericanCondition() { }
      // required for template inheritance
      public override FDVanillaEngine factory(GeneralizedBlackScholesProcess process,
                                              int timeSteps, int gridPoints, bool timeDependent)
      {
         return new FDAmericanCondition<baseEngine>(process, timeSteps, gridPoints, timeDependent);
      }

      public FDAmericanCondition(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
         : base(process, timeSteps, gridPoints, timeDependent)
      {
         engine_.setStepCondition(initializeStepConditionImpl);
      }

      protected IStepCondition<Vector> initializeStepConditionImpl()
      {
         return new AmericanCondition(engine_.intrinsicValues_.values());
      }
   }


   public class FDShoutCondition<baseEngine> : FDConditionTemplate<baseEngine>
      where baseEngine : FDConditionEngineTemplate, new ()
   {

      // required for generics
      public FDShoutCondition() { }
      // required for template inheritance
      public override FDVanillaEngine factory(GeneralizedBlackScholesProcess process,
                                              int timeSteps, int gridPoints, bool timeDependent)
      {
         return new FDShoutCondition<baseEngine>(process, timeSteps, gridPoints, timeDependent);
      }

      public FDShoutCondition(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
         : base(process, timeSteps, gridPoints, timeDependent)
      {
         engine_.setStepCondition(initializeStepConditionImpl);
      }

      protected IStepCondition<Vector> initializeStepConditionImpl()
      {
         // the following to rely on process_ which is the same for engine and here
         // therefore wrapping is not requried
         double residualTime = engine_.getResidualTime();
         double riskFreeRate = process_.riskFreeRate().link.zeroRate(residualTime, Compounding.Continuous).rate();

         return new ShoutCondition(engine_.intrinsicValues_.values(), residualTime, riskFreeRate);
      }
   }
}
