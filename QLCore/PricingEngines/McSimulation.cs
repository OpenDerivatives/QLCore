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
using System.Collections.Generic;
using System.Linq;

namespace QLCore
{
   //! base class for Monte Carlo engines
   /*! Eventually this class might offer greeks methods.  Deriving a
       class from McSimulation gives an easy way to write a Monte
       Carlo engine.

       See McVanillaEngine as an example.
   */
   public abstract class McSimulation<MC, RNG, S> where S : IGeneralStatistics, new ()
   {
      protected McSimulation(bool antitheticVariate, bool controlVariate)
      {
         antitheticVariate_ = antitheticVariate;
         controlVariate_ = controlVariate;
      }

      //! add samples until the required absolute tolerance is reached
      public double value(double tolerance, int maxSamples = int.MaxValue, int minSamples = 1023)
      {
         int sampleNumber = mcModel_.sampleAccumulator().samples();
         if (sampleNumber < minSamples)
         {
            mcModel_.addSamples(minSamples - sampleNumber);
            sampleNumber = mcModel_.sampleAccumulator().samples();
         }

         int nextBatch;
         double order;
         double error = mcModel_.sampleAccumulator().errorEstimate();
         while (maxError(error) > tolerance)
         {
            Utils.QL_REQUIRE(sampleNumber < maxSamples, () =>
                             "max number of samples (" + maxSamples
                             + ") reached, while error (" + error
                             + ") is still above tolerance (" + tolerance + ")");

            // conservative estimate of how many samples are needed
            order = maxError(error* error) / tolerance / tolerance;
            nextBatch = (int)Math.Max(sampleNumber* order * 0.8 - sampleNumber, minSamples);

            // do not exceed maxSamples
            nextBatch = Math.Min(nextBatch, maxSamples - sampleNumber);
            sampleNumber += nextBatch;
            mcModel_.addSamples(nextBatch);
            error = mcModel_.sampleAccumulator().errorEstimate();
         }

         return mcModel_.sampleAccumulator().mean();
      }

      //! simulate a fixed number of samples
      public double valueWithSamples(int samples)
      {
         int sampleNumber = mcModel_.sampleAccumulator().samples();

         Utils.QL_REQUIRE(samples >= sampleNumber, () =>
                          "number of already simulated samples (" + sampleNumber
                          + ") greater than requested samples (" + samples + ")");

         mcModel_.addSamples(samples - sampleNumber);

         return mcModel_.sampleAccumulator().mean();
      }

      //! error estimated using the samples simulated so far
      public double errorEstimate() { return mcModel_.sampleAccumulator().errorEstimate(); }

      //! access to the sample accumulator for richer statistics
      public S sampleAccumulator() { return mcModel_.sampleAccumulator(); }

      //! basic calculate method provided to inherited pricing engines
      public void calculate(double? requiredTolerance, int? requiredSamples, int? maxSamples)
      {
         Utils.QL_REQUIRE(requiredTolerance != null ||
                          requiredSamples != null, () => "neither tolerance nor number of samples set");

         //! Initialize the one-factor Monte Carlo
         if (this.controlVariate_)
         {
            double? controlVariateValue = this.controlVariateValue();
            Utils.QL_REQUIRE(controlVariateValue != null, () => "engine does not provide control-variation price");

            PathPricer<IPath> controlPP = this.controlPathPricer();
            Utils.QL_REQUIRE(controlPP != null, () => "engine does not provide control-variation path pricer");

            IPathGenerator<IRNG> controlPG = this.controlPathGenerator();

            this.mcModel_ = new MonteCarloModel<MC, RNG, S>(pathGenerator(), pathPricer(), FastActivator<S>.Create(), antitheticVariate_,
                                                            controlPP, controlVariateValue.GetValueOrDefault(), controlPG);
         }
         else
         {
            this.mcModel_ = new MonteCarloModel<MC, RNG, S>(pathGenerator(), pathPricer(), FastActivator<S>.Create(), antitheticVariate_);
         }

         if (requiredTolerance != null && requiredTolerance != Double.MaxValue)
         {
            if (maxSamples != null)
               value(requiredTolerance.Value, maxSamples.Value);
            else
               value(requiredTolerance.Value);
         }
         else
         {
            valueWithSamples(requiredSamples.GetValueOrDefault());
         }
      }


      protected abstract PathPricer<IPath> pathPricer();
      protected abstract IPathGenerator<IRNG> pathGenerator();
      protected abstract TimeGrid timeGrid();
      protected virtual PathPricer<IPath> controlPathPricer() { return null; }
      protected virtual IPathGenerator<IRNG> controlPathGenerator() { return null; }
      protected virtual IPricingEngine controlPricingEngine() { return null; }
      protected virtual double? controlVariateValue() { return null; }

      protected static double maxError(List<double> sequence) { return sequence.Max(); }
      protected static double maxError(double error) { return error; }

      protected MonteCarloModel<MC, RNG, S> mcModel_;
      protected bool antitheticVariate_, controlVariate_;
   }
}
