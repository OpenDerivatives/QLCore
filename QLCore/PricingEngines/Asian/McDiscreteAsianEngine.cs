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
using System.Linq;
using System;

namespace QLCore
{
   //! Pricing engine for discrete average Asians using Monte Carlo simulation
   /*! \warning control-variate calculation is disabled under VC++6.

       \ingroup asianengines
   */

   public class MCDiscreteAveragingAsianEngine<RNG, S> : McSimulation<SingleVariate, RNG, S>, IGenericEngine
   //DiscreteAveragingAsianOption.Engine,
   //McSimulation<SingleVariate,RNG,S>
      where RNG : IRSG, new ()
      where S : IGeneralStatistics, new ()
   {
      // data members
      protected GeneralizedBlackScholesProcess process_;
      protected int? timeStepsPerYear_;
      protected int? requiredSamples_, maxSamples_, timeSteps_;
      double? requiredTolerance_;
      bool brownianBridge_;
      ulong seed_;

      // constructor
      public MCDiscreteAveragingAsianEngine(
         GeneralizedBlackScholesProcess process,
         bool brownianBridge,
         bool antitheticVariate,
         bool controlVariate,
         int? requiredSamples,
         double? requiredTolerance,
         int? maxSamples,
         ulong seed,
         int? timeSteps = null,
         int? timeStepsPerYear = null) 
         : base(antitheticVariate, controlVariate)
      {
         process_ = process;
         timeSteps_ = timeSteps;
         timeStepsPerYear_ = timeStepsPerYear;
         requiredSamples_ = requiredSamples == null ? int.MaxValue : requiredSamples;
         maxSamples_ = maxSamples == null ? int.MaxValue : maxSamples;
         requiredTolerance_ = requiredTolerance == null ? Double.MaxValue : requiredTolerance_;
         brownianBridge_ = brownianBridge;
         seed_ = seed;
      }

      public void calculate()
      {
         base.calculate(requiredTolerance_, requiredSamples_, maxSamples_);
         
         results_.value = this.mcModel_.sampleAccumulator().mean();

         if (this.controlVariate_){
            // control variate might lead to small negative
                // option values for deep OTM options
                this.results_.value = Math.Max(0.0, this.results_.value.Value);
         }

         if (FastActivator<RNG>.Create().allowsErrorEstimate != 0)
            results_.errorEstimate =
               this.mcModel_.sampleAccumulator().errorEstimate();

         // Allow inspection of the timeGrid via additional results
         this.results_.additionalResults["TimeGrid"] = this.timeGrid();
      }

      // McSimulation implementation
      protected override TimeGrid timeGrid()
      {
         List<double> fixingTimes = new InitializedList<double>(arguments_.fixingDates.Count);
         for (int i = 0; i < arguments_.fixingDates.Count; i++)
         {
            double t = process_.time(arguments_.fixingDates[i]);
            
            if (t >= 0.0)
               fixingTimes[i] = t;
         }

         if (fixingTimes.empty() ||
            (fixingTimes.Count == 1 && fixingTimes.Last() == 0.0))
            Utils.QL_FAIL("all fixings are in the past", QLNetExceptionEnum.NullEffectiveDate);
         

         //Some models (eg. Heston) might request additional points in
         //the time grid to improve the accuracy of the discretization
         Date lastExerciseDate = this.arguments_.exercise.lastDate();
         double T = process_.time(lastExerciseDate);

         if (this.timeSteps_ != null)
            return new TimeGrid(fixingTimes, timeSteps_.Value);
         else if (this.timeStepsPerYear_ != null) 
            return new TimeGrid(fixingTimes, (int)(this.timeStepsPerYear_ * T));

        return new TimeGrid(fixingTimes);
      }

      protected override IPathGenerator<IRNG> pathGenerator()
      {
         int dimensions = process_.factors();
         TimeGrid grid = this.timeGrid();
         IRNG gen = (IRNG)new RNG()
               .make_sequence_generator(dimensions * (grid.size() - 1), seed_);
         return new PathGenerator<IRNG>(process_, grid,
                                        gen, brownianBridge_);
      }

      protected override double? controlVariateValue()
      {
         IPricingEngine controlPE = this.controlPricingEngine();
         Utils.QL_REQUIRE(controlPE != null, () => "engine does not provide control variation pricing engine");

         var controlArguments = controlPE.getArguments();

         (controlArguments as DiscreteAveragingAsianOption.Arguments).averageType 
               = arguments_.averageType;
         
         (controlArguments as DiscreteAveragingAsianOption.Arguments).exercise 
               = arguments_.exercise;

         (controlArguments as DiscreteAveragingAsianOption.Arguments).fixingDates 
               = arguments_.fixingDates;
         
         (controlArguments as DiscreteAveragingAsianOption.Arguments).pastFixings 
               = arguments_.pastFixings;

         (controlArguments as DiscreteAveragingAsianOption.Arguments).payoff 
               = arguments_.payoff;

         (controlArguments as DiscreteAveragingAsianOption.Arguments).runningAccumulator 
               = arguments_.runningAccumulator;

         controlPE.calculate();

         DiscreteAveragingAsianOption.Results controlResults =
            (DiscreteAveragingAsianOption.Results)(controlPE.getResults());

         return controlResults.value;
      }

      protected override PathPricer<IPath> pathPricer()
      {
         throw new System.NotImplementedException();
      }

      #region PricingEngine
      protected DiscreteAveragingAsianOption.Arguments arguments_ = new DiscreteAveragingAsianOption.Arguments();
      protected DiscreteAveragingAsianOption.Results results_ = new DiscreteAveragingAsianOption.Results();

      public IPricingEngineArguments getArguments() { return arguments_; }
      public IPricingEngineResults getResults() { return results_; }
      public void reset() { results_.reset(); }
      public void update() { process_.update(); }
      #endregion
   }
}
