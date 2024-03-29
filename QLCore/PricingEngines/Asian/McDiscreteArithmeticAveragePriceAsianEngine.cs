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

namespace QLCore
{
   //!  Monte Carlo pricing engine for discrete arithmetic average price Asian
   /*!  Monte Carlo pricing engine for discrete arithmetic average price
        Asian options. It can use MCDiscreteGeometricAPEngine (Monte Carlo
        discrete arithmetic average price engine) and
        AnalyticDiscreteGeometricAveragePriceAsianEngine (analytic discrete
        arithmetic average price engine) for control variation.

        \ingroup asianengines

        \test the correctness of the returned value is tested by
              reproducing results available in literature.
   */
   //template <class RNG = PseudoRandom, class S = Statistics>
   public class MCDiscreteArithmeticAPEngine<RNG, S>
      : MCDiscreteAveragingAsianEngine<RNG, S>
        where RNG : IRSG, new ()
        where S : IGeneralStatistics, new ()
   {

      // constructor
      public MCDiscreteArithmeticAPEngine(
         GeneralizedBlackScholesProcess process,
         bool brownianBridge,
         bool antitheticVariate,
         bool controlVariate,
         int? requiredSamples,
         double? requiredTolerance,
         int? maxSamples,
         ulong seed)
         : base(process, brownianBridge, antitheticVariate, controlVariate, 
                requiredSamples, requiredTolerance, maxSamples, seed)
      {
      }

      protected override PathPricer<IPath> pathPricer()
      {
         PlainVanillaPayoff payoff = (PlainVanillaPayoff)(this.arguments_.payoff);
         Utils.QL_REQUIRE(payoff != null, () => "non-plain payoff given");

         EuropeanExercise exercise = (EuropeanExercise)this.arguments_.exercise;
         Utils.QL_REQUIRE(exercise != null, () => "wrong exercise given");

         return (PathPricer<IPath>)new ArithmeticAPOPathPricer(
                   payoff.optionType(),
                   payoff.strike(),
                   this.process_.riskFreeRate().link.discount(this.timeGrid().Last()),
                   this.arguments_.runningAccumulator.GetValueOrDefault(),
                   this.arguments_.pastFixings.GetValueOrDefault());
      }

      protected override PathPricer<IPath> controlPathPricer()
      {
         PlainVanillaPayoff payoff = (PlainVanillaPayoff)this.arguments_.payoff;
         Utils.QL_REQUIRE(payoff != null, () => "non-plain payoff given");

         EuropeanExercise exercise = (EuropeanExercise)this.arguments_.exercise;
         Utils.QL_REQUIRE(exercise != null, () => "wrong exercise given");

         // for seasoned option the geometric strike might be rescaled
         // to obtain an equivalent arithmetic strike.
         // Any change applied here MUST be applied to the analytic engine too
         return (PathPricer<IPath>)new GeometricAPOPathPricer(
                   payoff.optionType(),
                   payoff.strike(),
                   this.process_.riskFreeRate().link.discount(this.timeGrid().Last()));
      }

      protected override IPricingEngine controlPricingEngine()
      {
         return new AnalyticDiscreteGeometricAveragePriceAsianEngine(this.process_);
      }
   }

   public class ArithmeticAPOPathPricer : PathPricer<IPath>
   {
      private PlainVanillaPayoff payoff_;
      private double discount_;
      private double runningSum_;
      private int pastFixings_;

      public ArithmeticAPOPathPricer(Option.Type type,
                                     double strike,
                                     double discount,
                                     double runningSum,
                                     int pastFixings)
      {
         payoff_ = new PlainVanillaPayoff(type, strike);
         discount_ = discount;
         runningSum_ = runningSum;
         pastFixings_ = pastFixings;
         Utils.QL_REQUIRE(strike >= 0.0, () => "strike less than zero not allowed");
      }

      public ArithmeticAPOPathPricer(Option.Type type,
                                     double strike,
                                     double discount,
                                     double runningSum)
         : this(type, strike, discount, runningSum, 0) { }

      public ArithmeticAPOPathPricer(Option.Type type,
                                     double strike,
                                     double discount)
         : this(type, strike, discount, 0.0, 0) { }


      public double value(IPath path)
      {
         int n = path.length();
         Utils.QL_REQUIRE(n > 1, () => "the path cannot be empty");

         double sum = runningSum_;
         int fixings;
         if ((path as Path).timeGrid().mandatoryTimes()[0].IsEqual(0.0))
         {
            // include initial fixing
            for (int i = 0; i < path.length(); i++)
               sum += (path as Path)[i];
            fixings = pastFixings_ + n;
         }
         else
         {
            for (int i = 1; i < path.length(); i++)
               sum += (path as Path)[i];
            fixings = pastFixings_ + n - 1;
         }
         double averagePrice = sum / fixings;
         return discount_ * payoff_.value(averagePrice);

      }
   }
   //<class RNG = PseudoRandom, class S = Statistics>
   public class MakeMCDiscreteArithmeticAPEngine<RNG, S>
      where RNG : IRSG, new ()
      where S : Statistics, new ()
   {
      public MakeMCDiscreteArithmeticAPEngine(GeneralizedBlackScholesProcess process)
      {
         process_ = process;
         antithetic_ = false;
         controlVariate_ = false;
         samples_ = null;
         maxSamples_ = null;
         tolerance_ = null;
         brownianBridge_ = true;
         seed_ = 0;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withBrownianBridge(bool b)
      {
         brownianBridge_ = b;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withBrownianBridge()
      {
         return withBrownianBridge(true);
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withSamples(int samples)
      {
         Utils.QL_REQUIRE(tolerance_ == null, () => "tolerance already set");
         samples_ = samples;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withTolerance(double tolerance)
      {
         Utils.QL_REQUIRE(samples_ == null, () => "number of samples already set");
         Utils.QL_REQUIRE(FastActivator<RNG>.Create().allowsErrorEstimate != 0, () =>
                          "chosen random generator policy does not allow an error estimate");
         tolerance_ = tolerance;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withMaxSamples(int samples)
      {
         maxSamples_ = samples;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withSeed(ulong seed)
      {
         seed_ = seed;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withAntitheticVariate(bool b)
      {
         antithetic_ = b;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withAntitheticVariate()
      {
         return this.withAntitheticVariate(true);
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withControlVariate(bool b)
      {
         controlVariate_ = b;
         return this;
      }

      public MakeMCDiscreteArithmeticAPEngine<RNG, S> withControlVariate()
      {
         return this.withControlVariate(true);
      }


      // conversion to pricing engine
      public IPricingEngine value()
      {
         return (IPricingEngine)new MCDiscreteArithmeticAPEngine<RNG, S>(process_,
                                                                         brownianBridge_,
                                                                         antithetic_, controlVariate_,
                                                                         samples_, tolerance_,
                                                                         maxSamples_,
                                                                         seed_);

      }

      private GeneralizedBlackScholesProcess process_;
      private bool antithetic_, controlVariate_;
      private int? samples_, maxSamples_;
      private double? tolerance_;
      private bool brownianBridge_;
      private ulong seed_;
   }

}
