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
   //! Pricing engine for vanilla options using Monte Carlo simulation
   /*! \ingroup vanillaengines */
   public abstract class MCVanillaEngine<MC, RNG, S> : MCVanillaEngine<MC, RNG, S, VanillaOption>
      where RNG : IRSG, new ()
      where S : IGeneralStatistics, new ()
   {
      protected MCVanillaEngine(StochasticProcess process,
                                int? timeSteps,
                                int? timeStepsPerYear,
                                bool brownianBridge,
                                bool antitheticVariate,
                                bool controlVariate,
                                int? requiredSamples,
                                double? requiredTolerance,
                                int? maxSamples,
                                ulong seed)
      : base(process, timeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, controlVariate, requiredSamples,
             requiredTolerance, maxSamples, seed) { }
   }

   public abstract class MCVanillaEngine<MC, RNG, S, Inst> : McSimulation<MC, RNG, S>, IGenericEngine
      where RNG : IRSG, new ()
      where S : IGeneralStatistics, new ()
   {
      protected StochasticProcess process_;
      protected int? timeSteps_, timeStepsPerYear_;
      protected int? requiredSamples_, maxSamples_;
      protected double? requiredTolerance_;
      protected bool brownianBridge_;
      protected ulong seed_;

      protected MCVanillaEngine(StochasticProcess process,
                                int? timeSteps,
                                int? timeStepsPerYear,
                                bool brownianBridge,
                                bool antitheticVariate,
                                bool controlVariate,
                                int? requiredSamples,
                                double? requiredTolerance,
                                int? maxSamples,
                                ulong seed)
      : base(antitheticVariate, controlVariate)
      {
         process_ = process;
         timeSteps_ = timeSteps;
         timeStepsPerYear_ = timeStepsPerYear;
         requiredSamples_ = requiredSamples;
         maxSamples_ = maxSamples;
         requiredTolerance_ = requiredTolerance;
         brownianBridge_ = brownianBridge;
         seed_ = seed;

         Utils.QL_REQUIRE(timeSteps != null || timeStepsPerYear != null, () => "no time steps provided");
         Utils.QL_REQUIRE(timeSteps == null || timeStepsPerYear == null, () =>
                          "both time steps and time steps per year were provided");
         if (timeSteps != null)
            Utils.QL_REQUIRE(timeSteps > 0, () => "timeSteps must be positive, " + timeSteps + " not allowed");
         if (timeStepsPerYear != null)
            Utils.QL_REQUIRE(timeStepsPerYear > 0, () =>
                             "timeStepsPerYear must be positive, " + timeStepsPerYear + " not allowed");
      }


      public virtual void calculate()
      {
         base.calculate(requiredTolerance_, requiredSamples_, maxSamples_);
         results_.value = mcModel_.sampleAccumulator().mean();
         if (FastActivator<RNG>.Create().allowsErrorEstimate != 0)
            results_.errorEstimate = mcModel_.sampleAccumulator().errorEstimate();
      }

      // McSimulation implementation
      protected override TimeGrid timeGrid()
      {
         Date lastExerciseDate = arguments_.exercise.lastDate();
         double t = process_.time(lastExerciseDate);
         if (timeSteps_ != null)
         {
            return new TimeGrid(t, timeSteps_.Value);
         }
         else if (timeStepsPerYear_ != null)
         {
            int steps = (int)(timeStepsPerYear_ * t);
            return new TimeGrid(t, Math.Max(steps, 1));
         }
         else
         {
            Utils.QL_FAIL("time steps not specified");
            return null;
         }
      }

      protected override IPathGenerator<IRNG> pathGenerator()
      {
         int dimensions = process_.factors();
         TimeGrid grid = timeGrid();
         IRNG generator = (IRNG) FastActivator<RNG>.Create().make_sequence_generator(dimensions * (grid.size() - 1), seed_);
         if (typeof(MC) == typeof(SingleVariate))
            return new PathGenerator<IRNG>(process_, grid, generator, brownianBridge_);

         return new MultiPathGenerator<IRNG>(process_, grid, generator, brownianBridge_);
      }

      protected override double? controlVariateValue()
      {
         AnalyticHestonHullWhiteEngine controlPE = controlPricingEngine() as AnalyticHestonHullWhiteEngine;
         Utils.QL_REQUIRE(controlPE != null, () => "engine does not provide control variation pricing engine");

         OneAssetOption.Arguments controlArguments = controlPE.getArguments() as VanillaOption.Arguments;
         Utils.QL_REQUIRE(controlArguments != null, () => "engine is using inconsistent arguments");

         controlPE.setupArguments(arguments_);
         controlPE.calculate();

         OneAssetOption.Results controlResults = controlPE.getResults() as VanillaOption.Results;
         Utils.QL_REQUIRE(controlResults != null, () => "engine returns an inconsistent result type");

         return controlResults.value;
      }

      #region PricingEngine
      protected OneAssetOption.Arguments arguments_ = new OneAssetOption.Arguments();
      protected OneAssetOption.Results results_ = new OneAssetOption.Results();

      public IPricingEngineArguments getArguments() { return arguments_; }
      public IPricingEngineResults getResults() { return results_; }
      public void reset() { results_.reset(); }
      public void update() { process_.update(); }

      #endregion
   }
}
